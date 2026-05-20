using System.Text.Json;
using LearningService.Application.Abstractions;
using Platform.Contracts.Events;

namespace LearningService.Infrastructure.Persistence;

public class EfOutboxWriter : IOutboxWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly LearningDbContext _db;

    public EfOutboxWriter(LearningDbContext db)
    {
        _db = db;
    }

    public async Task EnqueueAsync(CourseCompletedEvent evt, CancellationToken cancellationToken)
    {
        var entity = new OutboxMessage
        {
            Type = nameof(CourseCompletedEvent),
            Payload = JsonSerializer.Serialize(evt, JsonOptions)
        };

        await _db.OutboxMessages.AddAsync(entity, cancellationToken);
    }

    public async Task EnqueueAsync(LessonCompletedEvent evt, CancellationToken cancellationToken)
    {
        var entity = new OutboxMessage
        {
            Type = nameof(LessonCompletedEvent),
            Payload = JsonSerializer.Serialize(evt, JsonOptions)
        };

        await _db.OutboxMessages.AddAsync(entity, cancellationToken);
    }
}
