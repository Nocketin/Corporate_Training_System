using IdentityService.Application.Common.Abstractions;
using IdentityService.Domain;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure;

public class IdentityDbContext : DbContext, IUnitOfWork
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Email).IsUnique();
            b.Property(x => x.Email).IsRequired().HasMaxLength(256);
            b.Property(x => x.Username).IsRequired().HasMaxLength(100);
            b.Property(x => x.PasswordHash).IsRequired();
            b.Property(x => x.Role).HasConversion<string>().HasMaxLength(32).IsRequired();
        });

        modelBuilder.Entity<PasswordResetToken>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Token).IsUnique();
            b.Property(x => x.Token).IsRequired().HasMaxLength(128);
        });

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Type).HasMaxLength(500).IsRequired();
            b.Property(x => x.Payload).IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Token).IsUnique();
            b.Property(x => x.Token).IsRequired();
            b.Property(x => x.JwtId).IsRequired();
        });
    }
}

