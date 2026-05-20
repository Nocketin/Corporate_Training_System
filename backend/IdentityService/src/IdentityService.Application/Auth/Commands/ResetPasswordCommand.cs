using MediatR;

namespace IdentityService.Application.Auth.Commands;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest;
