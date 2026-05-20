using LearningService.Domain.Enums;

namespace LearningService.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? CompleteDate { get; set; }

    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
}
