using IdentityService.Application.Auth.Abstractions;
using IdentityService.Application.Auth.Commands;
using IdentityService.Application.Auth.Dtos;
using MediatR;

namespace IdentityService.Application.Auth.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return _tokenService.RefreshTokensAsync(request.AccessToken, request.RefreshToken, cancellationToken);
    }
}

