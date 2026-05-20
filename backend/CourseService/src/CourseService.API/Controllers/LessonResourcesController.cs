using CourseService.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.API.Controllers;

[ApiController]
[Route("api/lessons")]
public class LessonResourcesController : ControllerBase
{
    private readonly ILessonResourceReadRepository _resourceReadRepository;
    private readonly IObjectStorage _objectStorage;

    public LessonResourcesController(
        ILessonResourceReadRepository resourceReadRepository,
        IObjectStorage objectStorage)
    {
        _resourceReadRepository = resourceReadRepository;
        _objectStorage = objectStorage;
    }

    [HttpGet("{lessonId:guid}/resources/{resourceId:guid}/file")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFile(
        Guid lessonId,
        Guid resourceId,
        CancellationToken cancellationToken)
    {
        var resource = await _resourceReadRepository.GetFileResourceAsync(lessonId, resourceId, cancellationToken);
        if (resource is null
            || string.IsNullOrWhiteSpace(resource.StorageBucket)
            || string.IsNullOrWhiteSpace(resource.StorageKey))
        {
            return NotFound();
        }

        var obj = await _objectStorage.GetAsync(resource.StorageBucket, resource.StorageKey, cancellationToken);
        if (obj is null)
        {
            return NotFound();
        }

        var downloadName = string.IsNullOrWhiteSpace(resource.FileName) ? "material" : resource.FileName;
        return File(obj.Value.Content, obj.Value.ContentType, downloadName);
    }
}
