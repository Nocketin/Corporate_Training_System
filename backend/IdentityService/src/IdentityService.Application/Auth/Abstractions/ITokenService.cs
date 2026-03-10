using IdentityService.Application.Auth.Dtos;
using IdentityService.Domain;

namespace IdentityService.Application.Auth.Abstractions;

public interface ITokenService
{
    AuthResponse GenerateTokens(User user);

    Task<AuthResponse> RefreshTokensAsync(string accessToken, string refreshToken, CancellationToken cancellationToken);
}

