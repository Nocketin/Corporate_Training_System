using CourseService.Application.Courses.Dtos;
using CourseService.Domain.Entities;
using Mapster;

namespace CourseService.Application.Mapping;

public static class CourseMappingConfig
{
    public static void Register()
    {
        TypeAdapterConfig<Course, CourseListItemDto>.NewConfig()
            .Map(dest => dest.CoverImageUrl, src =>
                src.CoverImageBucket != null && src.CoverImageKey != null
                    ? $"/api/courses/{src.Id}/cover"
                    : null);

        TypeAdapterConfig<Course, CourseDetailDto>.NewConfig()
            .Map(dest => dest.Modules, src => src.Modules.OrderBy(m => m.Order).ToList());

        TypeAdapterConfig<Module, ModuleDto>.NewConfig()
            .Map(dest => dest.Lessons, src => src.Lessons.OrderBy(l => l.Order).ToList());

        TypeAdapterConfig<Lesson, LessonDto>.NewConfig()
            .Map(dest => dest.Resources, src => MapLessonResources(src));

        TypeAdapterConfig<LessonResource, LessonResourceDto>.NewConfig()
            .Map(dest => dest.Kind, src => src.Kind.ToString())
            .Map(dest => dest.DownloadUrl, src =>
                src.Kind == LessonResourceKind.File
                    ? $"/api/lessons/{src.LessonId}/resources/{src.Id}/file"
                    : null);
    }

    private static IReadOnlyList<LessonResourceDto> MapLessonResources(Lesson lesson)
    {
        var resources = lesson.Resources.OrderBy(r => r.Order).ToList();

        if (resources.Count == 0 && !string.IsNullOrWhiteSpace(lesson.ContentUrl))
        {
            return
            [
                new LessonResourceDto(
                    Guid.Empty,
                    LessonResourceKind.Link.ToString(),
                    "Дополнительный материал",
                    0,
                    lesson.ContentUrl,
                    null,
                    null,
                    null,
                    null)
            ];
        }

        return resources.Adapt<List<LessonResourceDto>>();
    }
}
