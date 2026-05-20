using LearningService.Application.Abstractions;
using LearningService.Application.Dtos;
using LearningService.Application.Learning.Queries;
using MediatR;

namespace LearningService.Application.Learning.Handlers;

public class GetMyEnrollmentsQueryHandler : IRequestHandler<GetMyEnrollmentsQuery, IReadOnlyList<MyEnrollmentDto>>
{
    private readonly ILearningRepository _repository;

    public GetMyEnrollmentsQueryHandler(ILearningRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MyEnrollmentDto>> Handle(
        GetMyEnrollmentsQuery request,
        CancellationToken cancellationToken)
    {
        var enrollments = await _repository.GetEnrollmentsForUserAsync(request.UserId, cancellationToken);
        var result = new List<MyEnrollmentDto>(enrollments.Count);

        foreach (var enrollment in enrollments)
        {
            var projection = await _repository.GetProjectionAsync(enrollment.CourseId, cancellationToken);
            var certificate = await _repository.GetCertificateAsync(
                request.UserId,
                enrollment.CourseId,
                cancellationToken);

            result.Add(new MyEnrollmentDto(
                enrollment.Id,
                enrollment.CourseId,
                projection?.Title,
                enrollment.Status,
                enrollment.StartDate,
                enrollment.CompleteDate,
                certificate?.FileUrl));
        }

        return result;
    }
}
