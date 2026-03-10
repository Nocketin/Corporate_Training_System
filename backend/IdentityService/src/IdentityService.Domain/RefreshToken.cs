namespace IdentityService.Domain;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    
    public string JwtId { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }

    public bool Used { get; set; }

    public bool Invalidated { get; set; }
}

