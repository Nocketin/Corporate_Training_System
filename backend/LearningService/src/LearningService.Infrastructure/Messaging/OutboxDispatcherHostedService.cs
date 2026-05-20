using System.Text.Json;
using Confluent.Kafka;
using LearningService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Contracts.Events;

namespace LearningService.Infrastructure.Messaging;

public class OutboxDispatcherHostedService : BackgroundService
{
    private const string CourseCompletedTopic = "course-completed";
    private const string LessonCompletedTopic = "lesson-completed";
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
                var db = scope.ServiceProvider.GetRequiredService<LearningDbContext>();

                var pending = await db.OutboxMessages
                    .Where(x => x.DispatchedAt == null)
                    .OrderBy(x => x.CreatedAt)
                    .Take(50)
                    .ToListAsync(stoppingToken)
                    .ConfigureAwait(false);

                foreach (var msg in pending)
                {
                    var dispatched = msg.Type switch
                    {
                        nameof(CourseCompletedEvent) => await TryDispatchCourseCompletedAsync(msg, stoppingToken),
                        nameof(LessonCompletedEvent) => await TryDispatchLessonCompletedAsync(msg, stoppingToken),
                        _ => true
                    };

                    if (dispatched)
                    {
                        msg.DispatchedAt = DateTime.UtcNow;
                    }
                }

                if (pending.Count > 0)
                {
                    await db.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
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

    private async Task<bool> TryDispatchCourseCompletedAsync(OutboxMessage msg, CancellationToken stoppingToken)
    {
        var evt = JsonSerializer.Deserialize<CourseCompletedEvent>(msg.Payload, JsonOptions);
        if (evt is null)
        {
            return true;
        }

        var body = JsonSerializer.Serialize(evt, JsonOptions);
        await _producer.ProduceAsync(
                CourseCompletedTopic,
                new Message<string, string> { Key = evt.CourseId.ToString(), Value = body },
                stoppingToken)
            .ConfigureAwait(false);
        return true;
    }

    private async Task<bool> TryDispatchLessonCompletedAsync(OutboxMessage msg, CancellationToken stoppingToken)
    {
        var evt = JsonSerializer.Deserialize<LessonCompletedEvent>(msg.Payload, JsonOptions);
        if (evt is null)
        {
            return true;
        }

        var body = JsonSerializer.Serialize(evt, JsonOptions);
        await _producer.ProduceAsync(
                LessonCompletedTopic,
                new Message<string, string> { Key = evt.LessonId.ToString(), Value = body },
                stoppingToken)
            .ConfigureAwait(false);
        return true;
    }
}
