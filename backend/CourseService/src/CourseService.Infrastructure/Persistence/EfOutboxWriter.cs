using System.Text.Json;
using CourseService.Application.Abstractions;
using Platform.Contracts.Events;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Infrastructure.Persistence;

public class EfOutboxWriter : IOutboxWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly CourseDbContext _db;

    public EfOutboxWriter(CourseDbContext db)
    {
        _db = db;
    }

    public async Task EnqueueAsync(CourseCreatedEvent evt, CancellationToken cancellationToken)
    {
        var entity = new OutboxMessage
        {
            Type = nameof(CourseCreatedEvent),
            Payload = JsonSerializer.Serialize(evt, JsonOptions)
        };

        await _db.OutboxMessages.AddAsync(entity, cancellationToken);
    }
}
