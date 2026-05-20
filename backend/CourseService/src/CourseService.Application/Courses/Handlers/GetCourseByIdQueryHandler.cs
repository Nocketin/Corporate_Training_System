using CourseService.Application.Abstractions;
using CourseService.Application.Courses.Dtos;
using CourseService.Application.Courses.Queries;
using Mapster;
using MediatR;

namespace CourseService.Application.Courses.Handlers;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDetailDto?>
{
    private readonly ICourseReadRepository _readRepository;

    public GetCourseByIdQueryHandler(ICourseReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<CourseDetailDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _readRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        return course?.Adapt<CourseDetailDto>();
    }
}
