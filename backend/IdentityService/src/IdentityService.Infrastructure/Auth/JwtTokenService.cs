using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Application.Auth.Abstractions;
using IdentityService.Application.Auth.Dtos;
using IdentityService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Infrastructure.Auth;

public class JwtTokenService : ITokenService
{
    private readonly IdentityDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public JwtTokenService(IdentityDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public AuthResponse GenerateTokens(User user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var accessMinutes = int.Parse(jwtSection["AccessTokenMinutes"] ?? "15");
        var refreshDays = int.Parse(jwtSection["RefreshTokenDays"] ?? "7");

        var handler = new JwtSecurityTokenHandler();
        var jwtId = Guid.NewGuid().ToString();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId)
            }),
            Expires = DateTime.UtcNow.AddMinutes(accessMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var token = handler.CreateToken(tokenDescriptor);
        var accessToken = handler.WriteToken(token);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            JwtId = jwtId,
            Token = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDays),
            Used = false,
            Invalidated = false
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.SaveChanges();

        return new AuthResponse(accessToken, refreshToken.Token);
    }

    public async Task<AuthResponse> RefreshTokensAsync(string accessToken, string refreshToken, CancellationToken cancellationToken)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];

        var handler = new JwtSecurityTokenHandler();
        SecurityToken? validatedToken;
        var principal = handler.ValidateToken(accessToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            ValidateLifetime = false
        }, out validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var jwtId = jwtToken.Id;
        
        var subClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
                       ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(subClaim))
        {
            throw new SecurityTokenException("Invalid access token: missing user identifier.");
        }
        
        var userId = Guid.Parse(subClaim);

        var storedRefresh = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

        if (storedRefresh is null ||
            storedRefresh.Used ||
            storedRefresh.Invalidated ||
            storedRefresh.ExpiresAt < DateTime.UtcNow ||
            storedRefresh.JwtId != jwtId ||
            storedRefresh.UserId != userId)
        {
            throw new SecurityTokenException("Invalid refresh token.");
        }

        storedRefresh.Used = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var user = await _dbContext.Users.SingleAsync(x => x.Id == userId, cancellationToken);
        return GenerateTokens(user);
    }
}

