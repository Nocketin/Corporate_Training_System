using IdentityService.Application.Auth.Commands;
using IdentityService.Application.Auth.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request.Email, request.Password), cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginUserCommand(request.Email, request.Password), cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request.AccessToken, request.RefreshToken), cancellationToken);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
        return Accepted();
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ResetPasswordCommand(request.Token, request.NewPassword), cancellationToken);
        return NoContent();
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IReadOnlyList<UserListItemDto>>> ListUsers(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListUsersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("users/{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserListItemDto>> UpdateRole(
        Guid id,
        [FromBody] UpdateUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new UpdateUserRoleCommand(id, request.Role), cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

public record RefreshRequest(string AccessToken, string RefreshToken);

