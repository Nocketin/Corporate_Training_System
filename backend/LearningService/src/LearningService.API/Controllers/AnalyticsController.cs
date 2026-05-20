using LearningService.Application.Analytics.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningService.API.Controllers;

[ApiController]
[Route("api/learning/analytics")]
[Authorize(Roles = "Admin")]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLearningAnalyticsOverviewQuery(from, to), cancellationToken);
        return Ok(result);
    }
}
