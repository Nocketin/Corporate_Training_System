namespace CourseService.Domain.Entities;

public class LessonResource : BaseEntity
{
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public LessonResourceKind Kind { get; set; }
    public string Title { get; set; } = null!;
    public int Order { get; set; }
    public string? Url { get; set; }
    public string? StorageBucket { get; set; }
    public string? StorageKey { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long? FileSizeBytes { get; set; }
}
