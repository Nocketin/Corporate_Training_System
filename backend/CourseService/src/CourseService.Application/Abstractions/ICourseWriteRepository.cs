using CourseService.Domain.Entities;

namespace CourseService.Application.Abstractions;

public interface ICourseWriteRepository
{
    Task AddAsync(Course course, CancellationToken cancellationToken);

    Task<Course?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken);

    Task DeleteAsync(Course course, CancellationToken cancellationToken);
}
