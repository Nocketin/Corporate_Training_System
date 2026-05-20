using CourseService.Application.Abstractions;
using CourseService.Application.Caching;
using CourseService.Application.Courses.Commands;
using CourseService.Domain.Entities;
using MediatR;

namespace CourseService.Application.Courses.Handlers;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand>
{
    private readonly ICourseWriteRepository _writeRepository;
    private readonly IObjectStorage _objectStorage;
    private readonly ICourseCacheInvalidator _cacheInvalidator;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourseCommandHandler(
        ICourseWriteRepository writeRepository,
        IObjectStorage objectStorage,
        ICourseCacheInvalidator cacheInvalidator,
        IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _objectStorage = objectStorage;
        _cacheInvalidator = cacheInvalidator;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _writeRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken)
                     ?? throw new KeyNotFoundException("Course not found.");

        await DeleteStorageObjectsAsync(course, cancellationToken);
        await _writeRepository.DeleteAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheInvalidator.InvalidateAllListsAsync(cancellationToken);
        await _cacheInvalidator.InvalidateCourseAsync(course.Id, cancellationToken);
    }

    private async Task DeleteStorageObjectsAsync(Course course, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(course.CoverImageBucket) &&
            !string.IsNullOrWhiteSpace(course.CoverImageKey))
        {
            await _objectStorage.DeleteAsync(course.CoverImageBucket, course.CoverImageKey, cancellationToken);
        }

        foreach (var module in course.Modules)
        {
            foreach (var lesson in module.Lessons)
            {
                foreach (var resource in lesson.Resources)
                {
                    if (!string.IsNullOrWhiteSpace(resource.StorageBucket) &&
                        !string.IsNullOrWhiteSpace(resource.StorageKey))
                    {
                        await _objectStorage.DeleteAsync(
                            resource.StorageBucket,
                            resource.StorageKey,
                            cancellationToken);
                    }
                }
            }
        }
    }
}
