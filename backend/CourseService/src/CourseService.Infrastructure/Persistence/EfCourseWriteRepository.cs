using CourseService.Application.Abstractions;
using CourseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Infrastructure.Persistence;

public class EfCourseWriteRepository : ICourseWriteRepository
{
    private readonly CourseDbContext _db;

    public EfCourseWriteRepository(CourseDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Course course, CancellationToken cancellationToken)
    {
        await _db.Courses.AddAsync(course, cancellationToken);
    }

    public Task<Course?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        return _db.Courses
            .Include(c => c.Modules)
            .ThenInclude(m => m.Lessons)
            .ThenInclude(l => l.Resources)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task DeleteAsync(Course course, CancellationToken cancellationToken)
    {
        _db.Courses.Remove(course);
        return Task.CompletedTask;
    }
}
