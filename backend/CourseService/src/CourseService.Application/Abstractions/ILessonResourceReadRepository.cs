using CourseService.Domain.Entities;

namespace CourseService.Application.Abstractions;

public interface ILessonResourceReadRepository
{
    Task<LessonResource?> GetFileResourceAsync(Guid lessonId, Guid resourceId, CancellationToken cancellationToken);
}
