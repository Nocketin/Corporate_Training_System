using CourseService.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace CourseService.Infrastructure.Caching;

public class RedisCourseCacheInvalidator : ICourseCacheInvalidator
{
    private readonly IDistributedCache _cache;

    public RedisCourseCacheInvalidator(IDistributedCache cache)
    {
        _cache = cache;
    }

    public Task InvalidateAllListsAsync(CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync("courses:list", cancellationToken);

    public Task InvalidateCourseAsync(Guid courseId, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync($"courses:item:{courseId}", cancellationToken);
}
