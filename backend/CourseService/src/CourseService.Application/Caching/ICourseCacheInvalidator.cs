namespace CourseService.Application.Caching;

public interface ICourseCacheInvalidator
{
    Task InvalidateAllListsAsync(CancellationToken cancellationToken = default);
    Task InvalidateCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
}
