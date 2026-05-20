using CourseService.Application.Abstractions;
using CourseService.Application.Caching;
using CourseService.Application.Courses.Commands;
using CourseService.Application.Courses.Dtos;
using CourseService.Application.Courses.Services;
using Platform.Contracts.Events;
using CourseService.Domain.Entities;
using Mapster;
using MediatR;
using Platform.Common.Observability;

namespace CourseService.Application.Courses.Handlers;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, CourseDetailDto>
{
    private readonly ICourseWriteRepository _writeRepository;
    private readonly IObjectStorage _objectStorage;
    private readonly IOutboxWriter _outboxWriter;
    private readonly ICourseCacheInvalidator _cacheInvalidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LessonResourceProcessor _resourceProcessor;

    public CreateCourseCommandHandler(
        ICourseWriteRepository writeRepository,
        IObjectStorage objectStorage,
        IOutboxWriter outboxWriter,
        ICourseCacheInvalidator cacheInvalidator,
        IUnitOfWork unitOfWork,
        LessonResourceProcessor resourceProcessor)
    {
        _writeRepository = writeRepository;
        _objectStorage = objectStorage;
        _outboxWriter = outboxWriter;
        _cacheInvalidator = cacheInvalidator;
        _unitOfWork = unitOfWork;
        _resourceProcessor = resourceProcessor;
    }

    public async Task<CourseDetailDto> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        string? bucket = null;
        string? key = null;

        if (request.CoverFile is not null &&
            !string.IsNullOrWhiteSpace(request.CoverFileName) &&
            !string.IsNullOrWhiteSpace(request.CoverContentType))
        {
            (bucket, key) = await _objectStorage.PutAsync(
                request.CoverFile,
                request.CoverFileName,
                request.CoverContentType,
                cancellationToken);
        }

        var course = new Course
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            AuthorId = request.AuthorId,
            Price = request.Price,
            CoverImageBucket = bucket,
            CoverImageKey = key
        };

        if (request.Modules is { Count: > 0 })
        {
            foreach (var m in request.Modules.OrderBy(x => x.Order))
            {
                var module = new Module
                {
                    Title = m.Title.Trim(),
                    Order = m.Order,
                    Course = course
                };

                foreach (var l in m.Lessons.OrderBy(x => x.Order))
                {
                    var lesson = new Lesson
                    {
                        Title = l.Title.Trim(),
                        ContentUrl = l.ContentUrl,
                        TextContent = l.TextContent,
                        DurationMinutes = l.DurationMinutes,
                        Order = l.Order,
                        Module = module
                    };

                    await _resourceProcessor.ApplyResourcesAsync(
                        lesson,
                        l.Resources,
                        request.LessonFiles,
                        cancellationToken);

                    module.Lessons.Add(lesson);
                }

                course.Modules.Add(module);
            }
        }

        await _writeRepository.AddAsync(course, cancellationToken);

        var evt = new CourseCreatedEvent(course.Id, course.Title, course.AuthorId, DateTime.UtcNow);
        await _outboxWriter.EnqueueAsync(evt, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        PlatformBusinessMetrics.RecordCourseCreated();

        await _cacheInvalidator.InvalidateAllListsAsync(cancellationToken);
        await _cacheInvalidator.InvalidateCourseAsync(course.Id, cancellationToken);

        return course.Adapt<CourseDetailDto>();
    }
}
