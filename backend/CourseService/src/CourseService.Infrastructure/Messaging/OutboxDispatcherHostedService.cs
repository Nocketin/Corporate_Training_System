using System.Text.Json;
using Confluent.Kafka;
using Platform.Contracts.Events;
using CourseService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CourseService.Infrastructure.Messaging;

public class OutboxDispatcherHostedService : BackgroundService
{
    private const string CoursePublishedTopic = "course-published";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxDispatcherHostedService> _logger;
    private readonly IProducer<string, string> _producer;

    public OutboxDispatcherHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxDispatcherHostedService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        var bootstrap = configuration["Kafka:Host"]
            ?? throw new InvalidOperationException("Configuration value Kafka:Host is required.");
        _producer = new ProducerBuilder<string, string>(new ProducerConfig
        {
            BootstrapServers = bootstrap,
            Acks = Acks.All,
            SocketTimeoutMs = 10_000,
            MessageTimeoutMs = 30_000,
        })
            .SetErrorHandler((_, e) => _logger.LogError("Kafka producer error: {Reason}", e.Reason))
            .Build();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await base.StopAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            try
            {
                _producer.Flush(TimeSpan.FromSeconds(10));
            }
            finally
            {
                _producer.Dispose();
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();

                var pending = await db.OutboxMessages
                    .Where(x => x.DispatchedAt == null)
                    .OrderBy(x => x.CreatedAt)
                    .Take(50)
                    .ToListAsync(stoppingToken)
                    .ConfigureAwait(false);

                foreach (var msg in pending)
                {
                    if (msg.Type != nameof(CourseCreatedEvent))
                    {
                        msg.DispatchedAt = DateTime.UtcNow;
                        continue;
                    }

                    var evt = JsonSerializer.Deserialize<CourseCreatedEvent>(msg.Payload, JsonOptions);
                    if (evt is null)
                    {
                        msg.DispatchedAt = DateTime.UtcNow;
                        continue;
                    }

                    var body = JsonSerializer.Serialize(evt, JsonOptions);
                    await _producer.ProduceAsync(
                            CoursePublishedTopic,
                            new Message<string, string> { Key = evt.CourseId.ToString(), Value = body },
                            stoppingToken)
                        .ConfigureAwait(false);
                    msg.DispatchedAt = DateTime.UtcNow;
                }

                if (pending.Count > 0)
                {
                    await db.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // shutting down
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox dispatch iteration failed");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }
}
