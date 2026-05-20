using MediatR;

namespace IdentityService.Application.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest;
