namespace LearningService.Application.Dtos;

public record CourseCatalogLessonDto(Guid Id, int Order);

public record CourseCatalogModuleDto(Guid Id, int Order, IReadOnlyList<CourseCatalogLessonDto> Lessons);

public record CourseCatalogDto(
    Guid Id,
    string Title,
    IReadOnlyList<CourseCatalogModuleDto> Modules);
