using Platform.Contracts.Events;

namespace IdentityService.Application.Auth.Abstractions;

public interface IOutboxWriter
{
    Task EnqueueAsync(UserCreatedEvent evt, CancellationToken cancellationToken);
}
