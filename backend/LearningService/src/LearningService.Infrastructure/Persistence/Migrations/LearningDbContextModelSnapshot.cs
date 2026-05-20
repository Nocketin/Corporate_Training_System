using System;
using LearningService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using LearningService.Domain.Entities;
using LearningService.Domain.Enums;

#nullable disable

namespace LearningService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(LearningDbContext))]
partial class LearningDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity<CourseProjection>(entity =>
        {
            entity.ToTable("CourseProjections");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.OrderedLessonIdsJson).HasColumnType("text");
            entity.HasIndex(e => e.CourseId).IsUnique();
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion(new EnumToStringConverter<EnrollmentStatus>()).HasMaxLength(32);
            entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();
        });

        modelBuilder.Entity<LessonProgress>(entity =>
        {
            entity.ToTable("LessonProgresses");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.EnrollmentId, e.LessonId }).IsUnique();
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasMaxLength(500);
            entity.Property(e => e.Payload).HasColumnType("text");
        });

        modelBuilder.Entity<ProcessedInboundMessage>(entity =>
        {
            entity.ToTable("ProcessedInboundMessages");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Topic, e.Partition, e.Offset }).IsUnique();
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.ToTable("Certificates");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileUrl).HasMaxLength(2000);
            entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();
        });
    }
}
