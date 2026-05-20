namespace LearningService.Domain.Entities;

public class ProcessedInboundMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Topic { get; set; } = null!;
    public int Partition { get; set; }
    public long Offset { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
