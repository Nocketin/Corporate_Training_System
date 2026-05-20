using LearningService.Application.Abstractions;
using LearningService.Application.Common;
using LearningService.Application.Dtos;
using LearningService.Application.Learning.Queries;
using MediatR;

namespace LearningService.Application.Learning.Handlers;

public class GetCourseProgressQueryHandler : IRequestHandler<GetCourseProgressQuery, CourseProgressDto>
{
    private readonly ILearningRepository _repository;
    private readonly ICourseProjectionSync _projectionSync;

    public GetCourseProgressQueryHandler(ILearningRepository repository, ICourseProjectionSync projectionSync)
    {
        _repository = repository;
        _projectionSync = projectionSync;
    }

    public async Task<CourseProgressDto> Handle(GetCourseProgressQuery request, CancellationToken cancellationToken)
    {
        await _projectionSync.SyncFromCatalogAsync(request.CourseId, cancellationToken);

        var projection = await _repository.GetProjectionAsync(request.CourseId, cancellationToken);
        var ordered = projection is null
            ? Array.Empty<Guid>()
            : LessonOrderHelper.FromJson(projection.OrderedLessonIdsJson);

        var enrollment = await _repository.GetEnrollmentWithProgressAsync(
            request.UserId,
            request.CourseId,
            cancellationToken);

        if (enrollment is null)
        {
            return new CourseProgressDto(
                request.CourseId,
                false,
                null,
                ordered.Count,
                0,
                0,
                Array.Empty<Guid>(),
                ordered.ToList(),
                ordered.FirstOrDefault());
        }

        var completed = enrollment.LessonProgresses
            .Where(p => p.IsCompleted)
            .Select(p => p.LessonId)
            .ToHashSet();

        var locked = new List<Guid>();
        Guid? nextLesson = null;
        for (var i = 0; i < ordered.Count; i++)
        {
            if (completed.Contains(ordered[i]))
            {
                continue;
            }

            var prerequisitesMet = ordered.Take(i).All(id => completed.Contains(id));
            if (prerequisitesMet)
            {
                nextLesson ??= ordered[i];
            }
            else
            {
                locked.Add(ordered[i]);
            }
        }

        var completedCount = completed.Count;
        var percent = ordered.Count == 0 ? 0 : (double)completedCount / ordered.Count * 100;

        return new CourseProgressDto(
            request.CourseId,
            true,
            enrollment.Status,
            ordered.Count,
            completedCount,
            percent,
            completed.ToList(),
            locked,
            nextLesson);
    }
}
