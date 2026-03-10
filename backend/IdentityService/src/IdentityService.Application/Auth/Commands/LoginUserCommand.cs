using IdentityService.Application.Auth.Dtos;
using MediatR;

namespace IdentityService.Application.Auth.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthResponse>;

