using MediatR;

namespace CourseService.Application.Caching;

public interface ICacheableRequest<out TResponse> : IRequest<TResponse>
{
    string CacheKey { get; }
    TimeSpan? AbsoluteExpiration { get; }
}
