namespace LearningService.Domain.Entities;

public class Certificate : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime IssueDate { get; set; }
    public string FileUrl { get; set; } = null!;
}
