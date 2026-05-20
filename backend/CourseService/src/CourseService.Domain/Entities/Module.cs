namespace CourseService.Domain.Entities;

public class Module : BaseEntity
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string Title { get; set; } = null!;
    public int Order { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
