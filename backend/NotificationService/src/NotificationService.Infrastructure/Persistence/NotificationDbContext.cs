using Microsoft.EntityFrameworkCore;

namespace NotificationService.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<ProcessedInboundMessage> ProcessedInboundMessages => Set<ProcessedInboundMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessedInboundMessage>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Topic, x.Partition, x.Offset }).IsUnique();
        });
    }
}
