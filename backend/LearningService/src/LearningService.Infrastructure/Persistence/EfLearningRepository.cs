using LearningService.Application.Abstractions;
using LearningService.Application.Common;
using LearningService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningService.Infrastructure.Persistence;

public class EfLearningRepository : ILearningRepository
{
    private readonly LearningDbContext _db;

    public EfLearningRepository(LearningDbContext db)
    {
        _db = db;
    }

    public Task<Enrollment?> GetEnrollmentAsync(Guid userId, Guid courseId, CancellationToken cancellationToken) =>
        _db.Enrollments.FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken);

    public Task<Enrollment?> GetEnrollmentWithProgressAsync(Guid userId, Guid courseId, CancellationToken cancellationToken) =>
        _db.Enrollments
            .Include(e => e.LessonProgresses)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken);

    public async Task<Guid?> FindCourseIdByLessonAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        var projections = await _db.CourseProjections.AsNoTracking().ToListAsync(cancellationToken);
        foreach (var projection in projections)
        {
            var ids = LessonOrderHelper.FromJson(projection.OrderedLessonIdsJson);
            if (ids.Contains(lessonId))
            {
                return projection.CourseId;
            }
        }

        return null;
    }

    public Task AddEnrollmentAsync(Enrollment enrollment, CancellationToken cancellationToken) =>
        _db.Enrollments.AddAsync(enrollment, cancellationToken).AsTask();

    public Task<CourseProjection?> GetProjectionAsync(Guid courseId, CancellationToken cancellationToken) =>
        _db.CourseProjections.FirstOrDefaultAsync(p => p.CourseId == courseId, cancellationToken);

    public async Task UpsertProjectionAsync(CourseProjection projection, CancellationToken cancellationToken)
    {
        var existing = await _db.CourseProjections
            .FirstOrDefaultAsync(p => p.CourseId == projection.CourseId, cancellationToken);

        if (existing is null)
        {
            await _db.CourseProjections.AddAsync(projection, cancellationToken);
            return;
        }

        existing.Title = projection.Title;
        existing.TotalLessons = projection.TotalLessons;
        existing.OrderedLessonIdsJson = projection.OrderedLessonIdsJson;
        existing.UpdateTimestamp();
    }

    public Task<bool> IsInboundProcessedAsync(string topic, int partition, long offset, CancellationToken cancellationToken) =>
        _db.ProcessedInboundMessages.AnyAsync(
            m => m.Topic == topic && m.Partition == partition && m.Offset == offset,
            cancellationToken);

    public Task MarkInboundProcessedAsync(string topic, int partition, long offset, CancellationToken cancellationToken)
    {
        _db.ProcessedInboundMessages.Add(new ProcessedInboundMessage
        {
            Topic = topic,
            Partition = partition,
            Offset = offset
        });
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Enrollment>> GetEnrollmentsForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var list = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(cancellationToken);
        return list;
    }

    public Task AddCertificateAsync(Certificate certificate, CancellationToken cancellationToken) =>
        _db.Certificates.AddAsync(certificate, cancellationToken).AsTask();

    public Task<Certificate?> GetCertificateAsync(Guid userId, Guid courseId, CancellationToken cancellationToken) =>
        _db.Certificates.FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId, cancellationToken);
}
