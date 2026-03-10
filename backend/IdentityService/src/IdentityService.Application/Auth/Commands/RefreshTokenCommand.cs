using IdentityService.Application.Auth.Dtos;
using MediatR;

namespace IdentityService.Application.Auth.Commands;


public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponse>;

