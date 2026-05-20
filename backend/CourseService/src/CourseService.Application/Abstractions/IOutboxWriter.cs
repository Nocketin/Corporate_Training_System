using Platform.Contracts.Events;

namespace CourseService.Application.Abstractions;

public interface IOutboxWriter
{
    Task EnqueueAsync(CourseCreatedEvent evt, CancellationToken cancellationToken);
}
