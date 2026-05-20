namespace LearningService.Domain.Entities;

public class CourseProjection : BaseEntity
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = null!;
    public int TotalLessons { get; set; }
    /// <summary>JSON array of lesson IDs in global order (module order, then lesson order).</summary>
    public string OrderedLessonIdsJson { get; set; } = "[]";
}
