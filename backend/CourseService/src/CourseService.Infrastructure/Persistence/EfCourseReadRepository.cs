using CourseService.Application.Abstractions;
using CourseService.Application.Courses.Dtos;
using CourseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Infrastructure.Persistence;

public class EfCourseReadRepository : ICourseReadRepository
{
    private readonly CourseDbContext _db;

    public EfCourseReadRepository(CourseDbContext db)
    {
        _db = db;
    }

    public async Task<List<CourseListItemDto>> ListAsync(CancellationToken cancellationToken)
    {
        return await _db.Courses
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CourseListItemDto(
                c.Id,
                c.Title,
                c.Description,
                c.AuthorId,
                c.Price,
                c.CoverImageBucket != null && c.CoverImageKey != null
                    ? $"/api/courses/{c.Id}/cover"
                    : null))
            .ToListAsync(cancellationToken);
    }

    public Task<Course?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return _db.Courses
            .AsNoTracking()
            .Include(c => c.Modules)
            .ThenInclude(m => m.Lessons)
            .ThenInclude(l => l.Resources)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
