using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NotificationService.Infrastructure.Persistence;

#nullable disable

namespace NotificationService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(NotificationDbContext))]
partial class NotificationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity<ProcessedInboundMessage>(entity =>
        {
            entity.ToTable("ProcessedInboundMessages");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Topic, e.Partition, e.Offset }).IsUnique();
        });
    }
}
