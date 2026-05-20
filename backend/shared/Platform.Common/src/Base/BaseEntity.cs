public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    public void UpdateTimestamp() => UpdatedAt = DateTime.UtcNow;

    /// <summary>Assigns fixed identity for migrations and seed data.</summary>
    public void AssignIdentity(Guid id, DateTime createdAtUtc)
    {
        Id = id;
        CreatedAt = createdAtUtc;
        UpdatedAt = createdAtUtc;
    }
}