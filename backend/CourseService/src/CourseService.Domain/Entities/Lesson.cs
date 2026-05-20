namespace CourseService.Domain.Entities;

public class Lesson : BaseEntity
{
    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? ContentUrl { get; set; }
    public string? TextContent { get; set; }
    public int DurationMinutes { get; set; }
    public int Order { get; set; }
    public ICollection<LessonResource> Resources { get; set; } = new List<LessonResource>();
}
