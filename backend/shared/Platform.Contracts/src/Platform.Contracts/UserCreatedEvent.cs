namespace Platform.Contracts.Events;

/// <summary>Published to Kafka topic <c>user-created</c> when a user registers.</summary>
public record UserCreatedEvent(Guid UserId, string Email, DateTime OccurredAtUtc);
