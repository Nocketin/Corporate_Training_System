namespace CourseService.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public decimal Price { get; set; }
    /// <summary>S3/MinIO bucket for cover image (optional).</summary>
    public string? CoverImageBucket { get; set; }
    /// <summary>Object key for cover image (optional).</summary>
    public string? CoverImageKey { get; set; }

    public ICollection<Module> Modules { get; set; } = new List<Module>();
}
