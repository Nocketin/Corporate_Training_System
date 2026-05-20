using LearningService.Application.Abstractions;
using LearningService.Domain.Entities;
using LearningService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LearningService.Infrastructure.Persistence;

public class LearningDbContext : DbContext, IUnitOfWork
{
    public LearningDbContext(DbContextOptions<LearningDbContext> options) : base(options)
    {
    }

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
    public DbSet<CourseProjection> CourseProjections => Set<CourseProjection>();
    public DbSet<ProcessedInboundMessage> ProcessedInboundMessages => Set<ProcessedInboundMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<Certificate> Certificates => Set<Certificate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
            e.HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();
            e.HasMany(x => x.LessonProgresses)
                .WithOne(x => x.Enrollment)
                .HasForeignKey(x => x.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LessonProgress>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.EnrollmentId, x.LessonId }).IsUnique();
        });

        modelBuilder.Entity<CourseProjection>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.CourseId).IsUnique();
            e.Property(x => x.Title).HasMaxLength(500);
            e.Property(x => x.OrderedLessonIdsJson).HasColumnType("text");
        });

        modelBuilder.Entity<ProcessedInboundMessage>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Topic, x.Partition, x.Offset }).IsUnique();
        });

        modelBuilder.Entity<OutboxMessage>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).HasMaxLength(500);
            e.Property(x => x.Payload).HasColumnType("text");
        });

        modelBuilder.Entity<Certificate>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();
            e.Property(x => x.FileUrl).HasMaxLength(2000).IsRequired();
        });

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entity.ClrType))
            {
                modelBuilder.Entity(entity.ClrType).Property(nameof(BaseEntity.Id)).ValueGeneratedNever();
            }
        }
    }

    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        base.SaveChangesAsync(cancellationToken);
}
