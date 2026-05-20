namespace Platform.Contracts.Events;

/// <summary>Published to Kafka topic <c>course-published</c> after course is persisted.</summary>
public record CourseCreatedEvent(Guid CourseId, string Title, Guid AuthorId, DateTime OccurredAtUtc);
