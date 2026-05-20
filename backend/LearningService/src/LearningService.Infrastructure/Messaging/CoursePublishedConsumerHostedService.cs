using System.Text.Json;
using Confluent.Kafka;
using LearningService.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Contracts.Events;

namespace LearningService.Infrastructure.Messaging;

public class CoursePublishedConsumerHostedService : BackgroundService
{
    private const string Topic = "course-published";
    private const string GroupId = "learning-service";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CoursePublishedConsumerHostedService> _logger;
    private readonly string _bootstrap;

    public CoursePublishedConsumerHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<CoursePublishedConsumerHostedService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _bootstrap = configuration["Kafka:Host"]
            ?? throw new InvalidOperationException("Configuration value Kafka:Host is required.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            IConsumer<string, string>? consumer = null;
            try
            {
                consumer = new ConsumerBuilder<string, string>(new ConsumerConfig
                {
                    BootstrapServers = _bootstrap,
                    GroupId = GroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false
                }).Build();

                consumer.Subscribe(Topic);
                _logger.LogInformation("Subscribed to {Topic} as {GroupId}", Topic, GroupId);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result.Message.Value is null)
                    {
                        consumer.Commit(result);
                        continue;
                    }

                    var evt = JsonSerializer.Deserialize<CourseCreatedEvent>(result.Message.Value, JsonOptions);
                    if (evt is null)
                    {
                        consumer.Commit(result);
                        continue;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<ILearningRepository>();
                    var sync = scope.ServiceProvider.GetRequiredService<ICourseProjectionSync>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    if (await repository.IsInboundProcessedAsync(
                            Topic,
                            result.Partition.Value,
                            result.Offset.Value,
                            stoppingToken))
                    {
                        consumer.Commit(result);
                        continue;
                    }

                    await sync.SyncFromCatalogAsync(evt.CourseId, stoppingToken);
                    await repository.MarkInboundProcessedAsync(
                        Topic,
                        result.Partition.Value,
                        result.Offset.Value,
                        stoppingToken);
                    await unitOfWork.SaveChangesAsync(stoppingToken);

                    consumer.Commit(result);
                    _logger.LogInformation("Processed course-published for {CourseId}", evt.CourseId);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka consumer error; retrying in 5s");
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
            finally
            {
                consumer?.Close();
                consumer?.Dispose();
            }
        }
    }
}
