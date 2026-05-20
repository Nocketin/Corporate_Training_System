namespace Platform.Contracts.Events;

/// <summary>Published to Kafka topic <c>course-completed</c> when a user finishes all lessons.</summary>
public record CourseCompletedEvent(Guid UserId, Guid CourseId, DateTime CompletedAtUtc);
