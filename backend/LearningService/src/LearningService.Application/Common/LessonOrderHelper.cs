using System.Text.Json;
using LearningService.Application.Dtos;

namespace LearningService.Application.Common;

public static class LessonOrderHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static IReadOnlyList<Guid> FromCatalog(CourseCatalogDto course)
    {
        return course.Modules
            .OrderBy(m => m.Order)
            .SelectMany(m => m.Lessons.OrderBy(l => l.Order).Select(l => l.Id))
            .ToList();
    }

    public static IReadOnlyList<Guid> FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Array.Empty<Guid>();
        }

        return JsonSerializer.Deserialize<List<Guid>>(json, JsonOptions) ?? new List<Guid>();
    }

    public static string ToJson(IReadOnlyList<Guid> lessonIds) =>
        JsonSerializer.Serialize(lessonIds, JsonOptions);
}
