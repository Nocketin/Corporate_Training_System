using CourseService.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CourseService.Infrastructure.Persistence;

/// <summary>
/// Fallback when demo rows are missing (e.g. migration not applied). Primary seed is EF migration <c>SeedDemoCourses</c>.
/// </summary>
public static class CourseDataSeeder
{
    public static readonly Guid DemoAuthorId = CourseDemoData.DemoAuthorId;

    public static async Task SeedAsync(CourseDbContext db, ILogger logger, CancellationToken cancellationToken = default)
    {
        var markerExists = await db.Courses.AnyAsync(c => c.Id == CourseDemoData.Course1Id, cancellationToken);
        if (markerExists)
        {
            return;
        }

        logger.LogInformation("Applying demo courses SQL (fallback seeder)…");

        await db.Database.ExecuteSqlRawAsync(CourseDemoSeedSql.Up, cancellationToken);

        logger.LogInformation("Inserted {Count} demo courses.", CourseDemoData.AllCourseIds.Count);
    }
}
