using LearningService.Application.Abstractions;
using LearningService.Application.Common;
using LearningService.Application.Dtos;
using LearningService.Application.Exceptions;
using LearningService.Application.Learning.Commands;
using LearningService.Domain.Entities;
using LearningService.Domain.Enums;
using MediatR;
using Platform.Common.Observability;
using Platform.Contracts.Events;

namespace LearningService.Application.Learning.Handlers;

public class CompleteLessonCommandHandler : IRequestHandler<CompleteLessonCommand, CompleteLessonResultDto>
{
    private readonly ILearningRepository _repository;
    private readonly ICourseProjectionSync _projectionSync;
    private readonly IOutboxWriter _outboxWriter;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteLessonCommandHandler(
        ILearningRepository repository,
        ICourseProjectionSync projectionSync,
        IOutboxWriter outboxWriter,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _projectionSync = projectionSync;
        _outboxWriter = outboxWriter;
        _unitOfWork = unitOfWork;
    }

    public async Task<CompleteLessonResultDto> Handle(CompleteLessonCommand request, CancellationToken cancellationToken)
    {
        var courseId = await _repository.FindCourseIdByLessonAsync(request.LessonId, cancellationToken);
        if (courseId is null)
        {
            throw new LessonOrderViolationException("Lesson is not known to the learning catalog.");
        }

        var enrollment = await _repository.GetEnrollmentWithProgressAsync(
            request.UserId,
            courseId.Value,
            cancellationToken);

        if (enrollment is null)
        {
            throw new NotEnrolledException(request.UserId, courseId.Value);
        }

        await _projectionSync.SyncFromCatalogAsync(enrollment.CourseId, cancellationToken);

        var projection = await _repository.GetProjectionAsync(enrollment.CourseId, cancellationToken);
        if (projection is null)
        {
            throw new CourseNotFoundException(enrollment.CourseId);
        }

        var ordered = LessonOrderHelper.FromJson(projection.OrderedLessonIdsJson);
        if (!ordered.Contains(request.LessonId))
        {
            throw new LessonOrderViolationException("Lesson does not belong to this course.");
        }

        var lessonIndex = ordered.ToList().IndexOf(request.LessonId);
        var completedIds = enrollment.LessonProgresses
            .Where(p => p.IsCompleted)
            .Select(p => p.LessonId)
            .ToHashSet();

        if (completedIds.Contains(request.LessonId))
        {
            return BuildResult(enrollment, request.LessonId, ordered, completedIds);
        }

        for (var i = 0; i < lessonIndex; i++)
        {
            if (!completedIds.Contains(ordered[i]))
            {
                throw new LessonOrderViolationException(
                    "Complete previous lessons before this one.");
            }
        }

        var progress = enrollment.LessonProgresses.FirstOrDefault(p => p.LessonId == request.LessonId);
        if (progress is null)
        {
            progress = new LessonProgress
            {
                EnrollmentId = enrollment.Id,
                LessonId = request.LessonId
            };
            enrollment.LessonProgresses.Add(progress);
        }

        progress.IsCompleted = true;
        progress.CompletedAt = DateTime.UtcNow;
        progress.UpdateTimestamp();
        completedIds.Add(request.LessonId);

        await _outboxWriter.EnqueueAsync(
            new LessonCompletedEvent(request.UserId, enrollment.CourseId, request.LessonId, DateTime.UtcNow),
            cancellationToken);

        var allDone = ordered.Count > 0 && ordered.All(id => completedIds.Contains(id));
        if (allDone && enrollment.Status != EnrollmentStatus.Completed)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompleteDate = DateTime.UtcNow;
            enrollment.UpdateTimestamp();

            await _outboxWriter.EnqueueAsync(
                new CourseCompletedEvent(request.UserId, enrollment.CourseId, DateTime.UtcNow),
                cancellationToken);

            var existingCert = await _repository.GetCertificateAsync(
                request.UserId,
                enrollment.CourseId,
                cancellationToken);
            if (existingCert is null)
            {
                var certificate = new Certificate
                {
                    UserId = request.UserId,
                    CourseId = enrollment.CourseId,
                    IssueDate = DateTime.UtcNow,
                    FileUrl = $"/api/learning/certificates/{enrollment.CourseId}"
                };
                await _repository.AddCertificateAsync(certificate, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        PlatformBusinessMetrics.RecordLessonCompleted();

        return BuildResult(enrollment, request.LessonId, ordered, completedIds);
    }

    private static CompleteLessonResultDto BuildResult(
        Enrollment enrollment,
        Guid lessonId,
        IReadOnlyList<Guid> ordered,
        HashSet<Guid> completedIds)
    {
        return new CompleteLessonResultDto(
            lessonId,
            enrollment.Status == EnrollmentStatus.Completed,
            enrollment.Status,
            completedIds.Count,
            ordered.Count);
    }
}
