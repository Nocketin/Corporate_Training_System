using CourseService.Application.Abstractions;
using CourseService.Application.Caching;
using CourseService.Application.Courses.Commands;
using CourseService.Application.Courses.Dtos;
using CourseService.Application.Courses.Services;
using CourseService.Domain.Entities;
using Mapster;
using MediatR;

namespace CourseService.Application.Courses.Handlers;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, CourseDetailDto>
{
    private readonly ICourseWriteRepository _writeRepository;
    private readonly IObjectStorage _objectStorage;
    private readonly ICourseCacheInvalidator _cacheInvalidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LessonResourceProcessor _resourceProcessor;

    public UpdateCourseCommandHandler(
        ICourseWriteRepository writeRepository,
        IObjectStorage objectStorage,
        ICourseCacheInvalidator cacheInvalidator,
        IUnitOfWork unitOfWork,
        LessonResourceProcessor resourceProcessor)
    {
        _writeRepository = writeRepository;
        _objectStorage = objectStorage;
        _cacheInvalidator = cacheInvalidator;
        _unitOfWork = unitOfWork;
        _resourceProcessor = resourceProcessor;
    }

    public async Task<CourseDetailDto> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _writeRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken)
                     ?? throw new KeyNotFoundException("Course not found.");

        course.Title = request.Title.Trim();
        course.Description = request.Description.Trim();
        course.Price = request.Price;
        course.UpdateTimestamp();

        if (request.CoverFile is not null &&
            !string.IsNullOrWhiteSpace(request.CoverFileName) &&
            !string.IsNullOrWhiteSpace(request.CoverContentType))
        {
            if (!string.IsNullOrWhiteSpace(course.CoverImageBucket) &&
                !string.IsNullOrWhiteSpace(course.CoverImageKey))
            {
                await _objectStorage.DeleteAsync(course.CoverImageBucket, course.CoverImageKey, cancellationToken);
            }

            var (bucket, key) = await _objectStorage.PutAsync(
                request.CoverFile,
                request.CoverFileName,
                request.CoverContentType,
                cancellationToken);
            course.CoverImageBucket = bucket;
            course.CoverImageKey = key;
        }

        await SyncModulesAsync(course, request.Modules, request.LessonFiles, request.DeletedResourceIds, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheInvalidator.InvalidateAllListsAsync(cancellationToken);
        await _cacheInvalidator.InvalidateCourseAsync(course.Id, cancellationToken);

        return course.Adapt<CourseDetailDto>();
    }

    private async Task SyncModulesAsync(
        Course course,
        IReadOnlyList<CreateModuleRequest>? modules,
        IReadOnlyDictionary<string, LessonFileUpload> lessonFiles,
        IReadOnlySet<Guid> deletedResourceIds,
        CancellationToken cancellationToken)
    {
        var incomingModules = modules ?? Array.Empty<CreateModuleRequest>();
        var keptModuleIds = new HashSet<Guid>();

        foreach (var m in incomingModules.OrderBy(x => x.Order))
        {
            var module = m.Id is { } moduleId
                ? course.Modules.FirstOrDefault(x => x.Id == moduleId)
                : null;

            if (module is null)
            {
                module = new Module { Course = course };
                course.Modules.Add(module);
            }

            module.Title = m.Title.Trim();
            module.Order = m.Order;
            module.UpdateTimestamp();
            keptModuleIds.Add(module.Id);

            await SyncLessonsAsync(module, m.Lessons, lessonFiles, deletedResourceIds, cancellationToken);
        }

        foreach (var removedModule in course.Modules.Where(m => !keptModuleIds.Contains(m.Id)).ToList())
        {
            foreach (var lesson in removedModule.Lessons)
            {
                await _resourceProcessor.DeleteResourcesAsync(lesson.Resources, cancellationToken);
            }

            course.Modules.Remove(removedModule);
        }
    }

    private async Task SyncLessonsAsync(
        Module module,
        IReadOnlyList<CreateLessonRequest> lessons,
        IReadOnlyDictionary<string, LessonFileUpload> lessonFiles,
        IReadOnlySet<Guid> deletedResourceIds,
        CancellationToken cancellationToken)
    {
        var keptLessonIds = new HashSet<Guid>();

        foreach (var l in lessons.OrderBy(x => x.Order))
        {
            var lesson = l.Id is { } lessonId
                ? module.Lessons.FirstOrDefault(x => x.Id == lessonId)
                : null;

            if (lesson is null)
            {
                lesson = new Lesson { Module = module };
                module.Lessons.Add(lesson);
            }

            lesson.Title = l.Title.Trim();
            lesson.ContentUrl = l.ContentUrl;
            lesson.TextContent = l.TextContent;
            lesson.DurationMinutes = l.DurationMinutes;
            lesson.Order = l.Order;
            lesson.UpdateTimestamp();
            keptLessonIds.Add(lesson.Id);

            await _resourceProcessor.SyncResourcesAsync(
                lesson,
                l.Resources,
                lessonFiles,
                deletedResourceIds,
                cancellationToken);
        }

        foreach (var removedLesson in module.Lessons.Where(l => !keptLessonIds.Contains(l.Id)).ToList())
        {
            await _resourceProcessor.DeleteResourcesAsync(removedLesson.Resources, cancellationToken);
            module.Lessons.Remove(removedLesson);
        }
    }
}
