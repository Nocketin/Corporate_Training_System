using CourseService.Application.Abstractions;
using CourseService.Application.Courses.Dtos;
using CourseService.Application.Courses.Queries;
using MediatR;

namespace CourseService.Application.Courses.Handlers;

public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, List<CourseListItemDto>>
{
    private readonly ICourseReadRepository _readRepository;

    public GetCoursesQueryHandler(ICourseReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public Task<List<CourseListItemDto>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        return _readRepository.ListAsync(cancellationToken);
    }
}
