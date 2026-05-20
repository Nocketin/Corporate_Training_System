using Platform.Contracts.Events;

namespace LearningService.Application.Abstractions;

public interface IOutboxWriter
{
    Task EnqueueAsync(CourseCompletedEvent evt, CancellationToken cancellationToken);

    Task EnqueueAsync(LessonCompletedEvent evt, CancellationToken cancellationToken);
}
