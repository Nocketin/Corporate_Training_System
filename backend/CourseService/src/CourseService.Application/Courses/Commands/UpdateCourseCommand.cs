using CourseService.Application.Courses.Dtos;
using MediatR;

namespace CourseService.Application.Courses.Commands;

public record UpdateCourseCommand(
    Guid CourseId,
    string Title,
    string Description,
    decimal Price,
    IReadOnlyList<CreateModuleRequest>? Modules,
    IReadOnlyDictionary<string, LessonFileUpload> LessonFiles,
    IReadOnlySet<Guid> DeletedResourceIds,
    Stream? CoverFile,
    string? CoverFileName,
    string? CoverContentType) : IRequest<CourseDetailDto>;
