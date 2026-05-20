using IdentityService.Application.Auth.Dtos;
using IdentityService.Domain;

namespace IdentityService.Application.Auth.Abstractions;

public interface ITokenService
{
    Task<AuthResponse> GenerateTokensAsync(User user, CancellationToken cancellationToken);

    Task<AuthResponse> RefreshTokensAsync(string accessToken, string refreshToken, CancellationToken cancellationToken);
}

