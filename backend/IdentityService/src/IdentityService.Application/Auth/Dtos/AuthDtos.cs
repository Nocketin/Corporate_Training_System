namespace IdentityService.Application.Auth.Dtos;

public record RegisterUserRequest(string Email, string Password);

public record LoginUserRequest(string Email, string Password);

public record AuthResponse(string AccessToken, string RefreshToken);

