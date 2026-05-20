using LearningService.Application.Abstractions;
using LearningService.Application.Dtos;
using LearningService.Application.Exceptions;
using LearningService.Application.Learning.Commands;
using LearningService.Domain.Entities;
using LearningService.Domain.Enums;
using MediatR;
using Platform.Common.Observability;

namespace LearningService.Application.Learning.Handlers;

public class EnrollInCourseCommandHandler : IRequestHandler<EnrollInCourseCommand, EnrollmentDto>
{
    private readonly ILearningRepository _repository;
    private readonly ICourseCatalogClient _catalog;
    private readonly ICourseProjectionSync _projectionSync;
    private readonly IUnitOfWork _unitOfWork;

    public EnrollInCourseCommandHandler(
        ILearningRepository repository,
        ICourseCatalogClient catalog,
        ICourseProjectionSync projectionSync,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _catalog = catalog;
        _projectionSync = projectionSync;
        _unitOfWork = unitOfWork;
    }

    public async Task<EnrollmentDto> Handle(EnrollInCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _catalog.GetCourseAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new CourseNotFoundException(request.CourseId);
        }

        await _projectionSync.SyncFromCatalogAsync(request.CourseId, cancellationToken);

        var existing = await _repository.GetEnrollmentAsync(request.UserId, request.CourseId, cancellationToken);
        if (existing is not null)
        {
            throw new AlreadyEnrolledException(request.UserId, request.CourseId);
        }

        var enrollment = new Enrollment
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            Status = EnrollmentStatus.Started,
            StartDate = DateTime.UtcNow
        };

        await _repository.AddEnrollmentAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        PlatformBusinessMetrics.RecordEnrollment();

        return new EnrollmentDto(
            enrollment.Id,
            enrollment.CourseId,
            enrollment.Status,
            enrollment.StartDate,
            enrollment.CompleteDate);
    }
}
