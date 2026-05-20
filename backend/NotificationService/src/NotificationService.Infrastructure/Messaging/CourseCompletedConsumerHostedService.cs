using System.Text.Json;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Infrastructure.Persistence;
using Platform.Contracts.Events;

namespace NotificationService.Infrastructure.Messaging;

public class CourseCompletedConsumerHostedService : BackgroundService
{
    private const string Topic = "course-completed";
    private const string GroupId = "notification-service";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CourseCompletedConsumerHostedService> _logger;
    private readonly string _bootstrap;

    public CourseCompletedConsumerHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<CourseCompletedConsumerHostedService> logger,
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

                    var evt = JsonSerializer.Deserialize<CourseCompletedEvent>(result.Message.Value, JsonOptions);
                    if (evt is null)
                    {
                        consumer.Commit(result);
                        continue;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

                    var alreadyProcessed = await db.ProcessedInboundMessages.AnyAsync(
                        m => m.Topic == Topic
                             && m.Partition == result.Partition.Value
                             && m.Offset == result.Offset.Value,
                        stoppingToken);

                    if (alreadyProcessed)
                    {
                        consumer.Commit(result);
                        continue;
                    }

                    _logger.LogInformation(
                        "Email отправлен пользователю {UserId}: поздравляем с завершением курса {CourseId} ({CompletedAtUtc:O})",
                        evt.UserId,
                        evt.CourseId,
                        evt.CompletedAtUtc);

                    db.ProcessedInboundMessages.Add(new ProcessedInboundMessage
                    {
                        Topic = Topic,
                        Partition = result.Partition.Value,
                        Offset = result.Offset.Value
                    });
                    await db.SaveChangesAsync(stoppingToken);

                    consumer.Commit(result);
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
