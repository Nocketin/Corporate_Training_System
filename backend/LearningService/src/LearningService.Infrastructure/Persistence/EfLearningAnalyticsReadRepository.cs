using LearningService.Application.Abstractions;
using LearningService.Application.Dtos;
using LearningService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LearningService.Infrastructure.Persistence;

public class EfLearningAnalyticsReadRepository : ILearningAnalyticsReadRepository
{
    private readonly LearningDbContext _db;

    public EfLearningAnalyticsReadRepository(LearningDbContext db)
    {
        _db = db;
    }

    public async Task<LearningAnalyticsOverviewDto> GetOverviewAsync(
        DateTime fromUtc,
        DateTime toUtcExclusive,
        CancellationToken cancellationToken)
    {
        var enrollmentsInRange = _db.Enrollments.AsNoTracking()
            .Where(e => e.StartDate >= fromUtc && e.StartDate < toUtcExclusive);

        var totalEnrollments = await enrollmentsInRange.CountAsync(cancellationToken);
        var completedEnrollments = await enrollmentsInRange
            .CountAsync(e => e.Status == EnrollmentStatus.Completed, cancellationToken);
        var activeEnrollments = await enrollmentsInRange
            .CountAsync(e => e.Status == EnrollmentStatus.Started, cancellationToken);

        var completionRate = totalEnrollments == 0
            ? 0
            : Math.Round((double)completedEnrollments / totalEnrollments * 100, 1);

        var enrollmentsByDayRaw = await enrollmentsInRange
            .GroupBy(e => e.StartDate.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        var enrollmentsByDay = enrollmentsByDayRaw
            .Select(x => new DailyCountDto(DateOnly.FromDateTime(x.Date), x.Count))
            .ToList();

        var completionsByDayRaw = await _db.Enrollments.AsNoTracking()
            .Where(e =>
                e.Status == EnrollmentStatus.Completed &&
                e.CompleteDate != null &&
                e.CompleteDate >= fromUtc &&
                e.CompleteDate < toUtcExclusive)
            .GroupBy(e => e.CompleteDate!.Value.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        var completionsByDay = completionsByDayRaw
            .Select(x => new DailyCountDto(DateOnly.FromDateTime(x.Date), x.Count))
            .ToList();

        var coursesActivityRaw = await _db.Enrollments.AsNoTracking()
            .Where(e => e.StartDate >= fromUtc && e.StartDate < toUtcExclusive)
            .GroupBy(e => e.CourseId)
            .Select(g => new
            {
                CourseId = g.Key,
                Enrollments = g.Count(),
                Completions = g.Count(e => e.Status == EnrollmentStatus.Completed)
            })
            .OrderByDescending(x => x.Enrollments)
            .Take(10)
            .ToListAsync(cancellationToken);

        var courseIds = coursesActivityRaw.Select(x => x.CourseId).ToList();
        var titlesByCourseId = await _db.CourseProjections.AsNoTracking()
            .Where(p => courseIds.Contains(p.CourseId))
            .ToDictionaryAsync(p => p.CourseId, p => p.Title, cancellationToken);

        var coursesActivity = coursesActivityRaw
            .Select(x => new CourseActivityDto(
                x.CourseId,
                titlesByCourseId.GetValueOrDefault(x.CourseId) ?? "Unknown",
                x.Enrollments,
                x.Completions))
            .ToList();

        return new LearningAnalyticsOverviewDto(
            new LearningAnalyticsSummaryDto(
                totalEnrollments,
                completedEnrollments,
                activeEnrollments,
                completionRate),
            enrollmentsByDay,
            completionsByDay,
            coursesActivity);
    }
}
