using System.Net.Http.Json;
using System.Text.Json;
using LearningService.Application.Abstractions;
using LearningService.Application.Dtos;

namespace LearningService.Infrastructure.Catalog;

public class CourseCatalogClient : ICourseCatalogClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _http;

    public CourseCatalogClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<CourseCatalogDto?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var response = await _http.GetAsync($"api/courses/{courseId}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var detail = JsonSerializer.Deserialize<CourseDetailResponse>(json, JsonOptions);
        if (detail is null)
        {
            return null;
        }

        var modules = detail.Modules
            .Select(m => new CourseCatalogModuleDto(
                m.Id,
                m.Order,
                m.Lessons.Select(l => new CourseCatalogLessonDto(l.Id, l.Order)).ToList()))
            .ToList();

        return new CourseCatalogDto(detail.Id, detail.Title, modules);
    }

    private sealed class CourseDetailResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public List<ModuleResponse> Modules { get; set; } = new();
    }

    private sealed class ModuleResponse
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public List<LessonResponse> Lessons { get; set; } = new();
    }

    private sealed class LessonResponse
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
    }
}
