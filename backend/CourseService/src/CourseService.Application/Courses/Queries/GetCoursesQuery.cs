using CourseService.Application.Caching;
using CourseService.Application.Courses.Dtos;
using MediatR;

namespace CourseService.Application.Courses.Queries;

public record GetCoursesQuery : IRequest<List<CourseListItemDto>>, ICacheableRequest<List<CourseListItemDto>>
{
    public string CacheKey => "courses:list";
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromMinutes(10);
}
