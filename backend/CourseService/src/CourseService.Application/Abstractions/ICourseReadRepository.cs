using CourseService.Application.Courses.Dtos;
using CourseService.Domain.Entities;

namespace CourseService.Application.Abstractions;

public interface ICourseReadRepository
{
    Task<List<CourseListItemDto>> ListAsync(CancellationToken cancellationToken);
    Task<Course?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
}
