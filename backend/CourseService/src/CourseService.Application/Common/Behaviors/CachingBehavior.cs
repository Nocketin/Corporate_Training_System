using System.Text.Json;
using CourseService.Application.Caching;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CourseService.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheableRequest<TResponse> cacheable)
        {
            return await next();
        }

        try
        {
            var cached = await _cache.GetStringAsync(cacheable.CacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cached))
            {
                var deserialized = JsonSerializer.Deserialize<TResponse>(cached, JsonOptions);
                if (deserialized is not null)
                {
                    return deserialized;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache read failed for {CacheKey}; executing handler without cache.", cacheable.CacheKey);
        }

        var response = await next();

        try
        {
            var serialized = JsonSerializer.Serialize(response, JsonOptions);
            await _cache.SetStringAsync(
                cacheable.CacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheable.AbsoluteExpiration
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache write failed for {CacheKey}; returning uncached response.", cacheable.CacheKey);
        }

        return response;
    }
}
