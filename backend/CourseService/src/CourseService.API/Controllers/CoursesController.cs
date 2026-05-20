using System.Text.Json;
using CourseService.Application.Abstractions;
using CourseService.Application.Courses.Commands;
using CourseService.Application.Courses.Dtos;
using CourseService.Application.Courses.Queries;
using CourseService.API.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IMediator _mediator;
    private readonly ICourseReadRepository _readRepository;
    private readonly IObjectStorage _objectStorage;

    public CoursesController(
        IMediator mediator,
        ICourseReadRepository readRepository,
        IObjectStorage objectStorage)
    {
        _mediator = mediator;
        _readRepository = readRepository;
        _objectStorage = objectStorage;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CourseListItemDto>>> List(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCoursesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<CourseDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCourseByIdQuery(id), cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}/cover")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCover(Guid id, CancellationToken cancellationToken)
    {
        var course = await _readRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (course is null || course.CoverImageBucket is null || course.CoverImageKey is null)
        {
            return NotFound();
        }

        var obj = await _objectStorage.GetAsync(course.CoverImageBucket, course.CoverImageKey, cancellationToken);
        if (obj is null)
        {
            return NotFound();
        }

        return File(obj.Value.Content, obj.Value.ContentType);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CourseDetailDto>> Create(
        [FromForm] string title,
        [FromForm] string description,
        [FromForm] Guid authorId,
        [FromForm] decimal price,
        [FromForm] string? modulesJson,
        IFormFile? cover,
        CancellationToken cancellationToken)
    {
        var modules = DeserializeModules(modulesJson);
        var lessonFiles = await MultipartLessonFiles.ParseAsync(Request.Form.Files, cancellationToken);

        Stream? coverStream = null;
        string? coverName = null;
        string? coverContentType = null;
        if (cover is { Length: > 0 })
        {
            coverStream = cover.OpenReadStream();
            coverName = cover.FileName;
            coverContentType = cover.ContentType;
        }

        try
        {
            var command = new CreateCourseCommand(
                title,
                description,
                authorId,
                price,
                modules,
                lessonFiles,
                coverStream,
                coverName,
                coverContentType);

            var created = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        finally
        {
            if (coverStream is not null)
            {
                await coverStream.DisposeAsync();
            }

            foreach (var upload in lessonFiles.Values)
            {
                await upload.Content.DisposeAsync();
            }
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new DeleteCourseCommand(id), cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CourseDetailDto>> Update(
        Guid id,
        [FromForm] string title,
        [FromForm] string description,
        [FromForm] decimal price,
        [FromForm] string? modulesJson,
        [FromForm] string? deletedResourceIds,
        IFormFile? cover,
        CancellationToken cancellationToken)
    {
        var modules = DeserializeModules(modulesJson);
        var lessonFiles = await MultipartLessonFiles.ParseAsync(Request.Form.Files, cancellationToken);
        var deletedIds = DeserializeDeletedResourceIds(deletedResourceIds);

        Stream? coverStream = null;
        string? coverName = null;
        string? coverContentType = null;
        if (cover is { Length: > 0 })
        {
            coverStream = cover.OpenReadStream();
            coverName = cover.FileName;
            coverContentType = cover.ContentType;
        }

        try
        {
            var command = new UpdateCourseCommand(
                id,
                title,
                description,
                price,
                modules,
                lessonFiles,
                deletedIds,
                coverStream,
                coverName,
                coverContentType);

            var updated = await _mediator.Send(command, cancellationToken);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        finally
        {
            if (coverStream is not null)
            {
                await coverStream.DisposeAsync();
            }

            foreach (var upload in lessonFiles.Values)
            {
                await upload.Content.DisposeAsync();
            }
        }
    }

    private static IReadOnlyList<CreateModuleRequest>? DeserializeModules(string? modulesJson)
    {
        if (string.IsNullOrWhiteSpace(modulesJson))
        {
            return null;
        }

        return JsonSerializer.Deserialize<IReadOnlyList<CreateModuleRequest>>(modulesJson, JsonOptions);
    }

    private static IReadOnlySet<Guid> DeserializeDeletedResourceIds(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new HashSet<Guid>();
        }

        var ids = JsonSerializer.Deserialize<List<Guid>>(json, JsonOptions);
        return ids is null ? new HashSet<Guid>() : ids.ToHashSet();
    }
}
