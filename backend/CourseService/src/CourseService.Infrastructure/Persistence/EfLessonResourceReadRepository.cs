using CourseService.Application.Abstractions;
using CourseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Infrastructure.Persistence;

public class EfLessonResourceReadRepository : ILessonResourceReadRepository
{
    private readonly CourseDbContext _db;

    public EfLessonResourceReadRepository(CourseDbContext db)
    {
        _db = db;
    }

    public Task<LessonResource?> GetFileResourceAsync(Guid lessonId, Guid resourceId, CancellationToken cancellationToken)
    {
        return _db.LessonResources
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.Id == resourceId
                     && r.LessonId == lessonId
                     && r.Kind == LessonResourceKind.File,
                cancellationToken);
    }
}
