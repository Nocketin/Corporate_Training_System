using LearningService.Application.Dtos;

namespace LearningService.Application.Abstractions;

public interface ICourseCatalogClient
{
    Task<CourseCatalogDto?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken);
}
