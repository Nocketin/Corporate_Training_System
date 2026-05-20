using CourseService.Application.Caching;
using CourseService.Application.Courses.Dtos;
using MediatR;

namespace CourseService.Application.Courses.Queries;

public record GetCourseByIdQuery(Guid Id) : IRequest<CourseDetailDto?>, ICacheableRequest<CourseDetailDto?>
{
    public string CacheKey => $"courses:item:{Id}";
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromMinutes(10);
}
