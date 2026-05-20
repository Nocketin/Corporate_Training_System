namespace CourseService.Application.Courses.Dtos;

public record CourseListItemDto(Guid Id, string Title, string Description, Guid AuthorId, decimal Price, string? CoverImageUrl);

public record LessonResourceDto(
    Guid Id,
    string Kind,
    string Title,
    int Order,
    string? Url,
    string? FileName,
    string? ContentType,
    long? FileSizeBytes,
    string? DownloadUrl);

public record LessonDto(
    Guid Id,
    string Title,
    string? ContentUrl,
    string? TextContent,
    int DurationMinutes,
    int Order,
    IReadOnlyList<LessonResourceDto> Resources);

public record ModuleDto(Guid Id, string Title, int Order, IReadOnlyList<LessonDto> Lessons);

public record CourseDetailDto(
    Guid Id,
    string Title,
    string Description,
    Guid AuthorId,
    decimal Price,
    string? CoverImageBucket,
    string? CoverImageKey,
    IReadOnlyList<ModuleDto> Modules);

public record CreateLessonResourceRequest(
    Guid? Id,
    string Kind,
    string Title,
    int Order,
    string? Url,
    string? FileKey);

public record CreateLessonRequest(
    Guid? Id,
    string Title,
    string? ContentUrl,
    string? TextContent,
    int DurationMinutes,
    int Order,
    IReadOnlyList<CreateLessonResourceRequest>? Resources);

public record CreateModuleRequest(
    Guid? Id,
    string Title,
    int Order,
    IReadOnlyList<CreateLessonRequest> Lessons);
