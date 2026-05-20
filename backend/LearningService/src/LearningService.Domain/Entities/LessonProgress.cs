namespace LearningService.Domain.Entities;

public class LessonProgress : BaseEntity
{
    public Guid EnrollmentId { get; set; }
    public Guid LessonId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Enrollment Enrollment { get; set; } = null!;
}
