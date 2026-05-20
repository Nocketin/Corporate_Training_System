using CourseService.Application.Courses.Dtos;
using MediatR;

namespace CourseService.Application.Courses.Commands;

public record CreateCourseCommand(
    string Title,
    string Description,
    Guid AuthorId,
    decimal Price,
    IReadOnlyList<CreateModuleRequest>? Modules,
    IReadOnlyDictionary<string, LessonFileUpload> LessonFiles,
    Stream? CoverFile,
    string? CoverFileName,
    string? CoverContentType) : IRequest<CourseDetailDto>;
