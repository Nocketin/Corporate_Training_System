using LearningService.Application.Abstractions;
using LearningService.Application.Common;
using LearningService.Domain.Entities;

namespace LearningService.Infrastructure.Catalog;

public class CourseProjectionSync : ICourseProjectionSync
{
    private readonly ICourseCatalogClient _catalog;
    private readonly ILearningRepository _repository;
    private readonly LearningService.Application.Abstractions.IUnitOfWork _unitOfWork;

    public CourseProjectionSync(
        ICourseCatalogClient catalog,
        ILearningRepository repository,
        LearningService.Application.Abstractions.IUnitOfWork unitOfWork)
    {
        _catalog = catalog;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task SyncFromCatalogAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await _catalog.GetCourseAsync(courseId, cancellationToken);
        if (course is null)
        {
            return;
        }

        var ordered = LessonOrderHelper.FromCatalog(course);
        var projection = new CourseProjection
        {
            CourseId = course.Id,
            Title = course.Title,
            TotalLessons = ordered.Count,
            OrderedLessonIdsJson = LessonOrderHelper.ToJson(ordered)
        };

        var existing = await _repository.GetProjectionAsync(courseId, cancellationToken);
        if (existing is not null)
        {
            existing.Title = projection.Title;
            existing.TotalLessons = projection.TotalLessons;
            existing.OrderedLessonIdsJson = projection.OrderedLessonIdsJson;
            existing.UpdateTimestamp();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        await _repository.UpsertProjectionAsync(projection, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
