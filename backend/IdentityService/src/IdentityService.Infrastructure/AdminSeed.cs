using IdentityService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure;

public static class AdminSeed
{
    public static async Task SeedAsync(IdentityDbContext db, IConfiguration configuration, ILogger logger, CancellationToken cancellationToken = default)
    {
        var adminEmail = configuration["AdminSeed:Email"];
        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            return;
        }

        var normalized = adminEmail.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.Email.ToLower() == normalized,
            cancellationToken);
        if (user is null)
        {
            return;
        }

        if (user.Role == UserRole.Admin)
        {
            return;
        }

        user.Role = UserRole.Admin;
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Promoted user {Email} to Admin role via AdminSeed.", user.Email);
    }
}
