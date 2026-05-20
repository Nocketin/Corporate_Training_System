namespace IdentityService.Application.Auth.Dtos;

public record RegisterUserRequest(string Email, string Password);

public record LoginUserRequest(string Email, string Password);

public record AuthResponse(string AccessToken, string RefreshToken, string Role);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Token, string NewPassword);

public record UserListItemDto(Guid Id, string Email, string Username, string Role, DateTime CreatedAt);

public record UpdateUserRoleRequest(string Role);

