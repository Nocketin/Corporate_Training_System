using CourseService.Application.Abstractions;
using CourseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Infrastructure.Persistence;

public class CourseDbContext : DbContext, IUnitOfWork
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonResource> LessonResources => Set<LessonResource>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(e =>
        {
            e.HasKey(x => x.Id);
            // Client-generated Guids (BaseEntity); avoid EF omitting Id on INSERT when DB has no uuid default.
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Title).HasMaxLength(500).IsRequired();
            e.Property(x => x.Description).HasMaxLength(8000).IsRequired();
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.HasMany(x => x.Modules)
                .WithOne(x => x.Course)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Module>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Title).HasMaxLength(500).IsRequired();
            e.HasMany(x => x.Lessons)
                .WithOne(x => x.Module)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Lesson>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Title).HasMaxLength(500).IsRequired();
            e.Property(x => x.ContentUrl).HasMaxLength(2000);
            e.Property(x => x.TextContent).HasMaxLength(20000);
            e.HasMany(x => x.Resources)
                .WithOne(x => x.Lesson)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LessonResource>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Title).HasMaxLength(500).IsRequired();
            e.Property(x => x.Url).HasMaxLength(2000);
            e.Property(x => x.StorageBucket).HasMaxLength(200);
            e.Property(x => x.StorageKey).HasMaxLength(500);
            e.Property(x => x.FileName).HasMaxLength(500);
            e.Property(x => x.ContentType).HasMaxLength(200);
            e.Property(x => x.Kind).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<OutboxMessage>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Type).HasMaxLength(500).IsRequired();
            e.Property(x => x.Payload).IsRequired();
            e.HasIndex(x => x.DispatchedAt);
        });
    }
}
