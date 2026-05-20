using System.Text.Json;
using IdentityService.Application.Auth.Abstractions;
using Platform.Contracts.Events;

namespace IdentityService.Infrastructure.Persistence;

public class EfOutboxWriter : IOutboxWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IdentityDbContext _db;

    public EfOutboxWriter(IdentityDbContext db)
    {
        _db = db;
    }

    public async Task EnqueueAsync(UserCreatedEvent evt, CancellationToken cancellationToken)
    {
        var entity = new OutboxMessage
        {
            Type = nameof(UserCreatedEvent),
            Payload = JsonSerializer.Serialize(evt, JsonOptions)
        };

        await _db.OutboxMessages.AddAsync(entity, cancellationToken);
    }
}
