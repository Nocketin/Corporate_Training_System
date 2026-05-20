namespace CourseService.Application.Courses.Dtos;

public record LessonFileUpload(
    string FileKey,
    Stream Content,
    string FileName,
    string? ContentType,
    long Length);
