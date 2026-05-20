namespace Platform.Contracts.Events;

/// <summary>Published to Kafka topic <c>lesson-completed</c> when a user completes a lesson.</summary>
public record LessonCompletedEvent(Guid UserId, Guid CourseId, Guid LessonId, DateTime OccurredAtUtc);
