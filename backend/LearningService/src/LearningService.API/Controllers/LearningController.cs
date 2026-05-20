using LearningService.API.Services;
using LearningService.Application.Learning.Commands;
using LearningService.Application.Learning.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningService.API.Controllers;

[ApiController]
[Route("api/learning")]
[Authorize]
public class LearningController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public LearningController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpPost("enroll/{courseId:guid}")]
    public async Task<IActionResult> Enroll(Guid courseId, CancellationToken cancellationToken)
    {
        var userId = RequireUserId();
        var result = await _mediator.Send(new EnrollInCourseCommand(courseId, userId), cancellationToken);
        return CreatedAtAction(nameof(GetProgress), new { courseId }, result);
    }

    [HttpPost("lessons/{lessonId:guid}/complete")]
    public async Task<IActionResult> CompleteLesson(Guid lessonId, CancellationToken cancellationToken)
    {
        var userId = RequireUserId();
        var result = await _mediator.Send(new CompleteLessonCommand(lessonId, userId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("courses/{courseId:guid}/progress")]
    public async Task<IActionResult> GetProgress(Guid courseId, CancellationToken cancellationToken)
    {
        var userId = RequireUserId();
        var result = await _mediator.Send(new GetCourseProgressQuery(courseId, userId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("my-enrollments")]
    public async Task<IActionResult> GetMyEnrollments(CancellationToken cancellationToken)
    {
        var userId = RequireUserId();
        var result = await _mediator.Send(new GetMyEnrollmentsQuery(userId), cancellationToken);
        return Ok(result);
    }

    private Guid RequireUserId()
    {
        if (_currentUser.UserId is { } id)
        {
            return id;
        }

        throw new UnauthorizedAccessException("User is not authenticated.");
    }
}
