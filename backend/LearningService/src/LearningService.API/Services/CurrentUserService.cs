using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LearningService.API.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var sub = user?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
}
