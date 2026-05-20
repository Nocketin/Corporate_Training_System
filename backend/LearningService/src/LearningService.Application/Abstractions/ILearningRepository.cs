using LearningService.Domain.Entities;

namespace LearningService.Application.Abstractions;

public interface ILearningRepository
{
    Task<Enrollment?> GetEnrollmentAsync(Guid userId, Guid courseId, CancellationToken cancellationToken);
    Task<Guid?> FindCourseIdByLessonAsync(Guid lessonId, CancellationToken cancellationToken);
    Task<Enrollment?> GetEnrollmentWithProgressAsync(Guid userId, Guid courseId, CancellationToken cancellationToken);
    Task AddEnrollmentAsync(Enrollment enrollment, CancellationToken cancellationToken);
    Task<CourseProjection?> GetProjectionAsync(Guid courseId, CancellationToken cancellationToken);
    Task UpsertProjectionAsync(CourseProjection projection, CancellationToken cancellationToken);
    Task<bool> IsInboundProcessedAsync(string topic, int partition, long offset, CancellationToken cancellationToken);
    Task MarkInboundProcessedAsync(string topic, int partition, long offset, CancellationToken cancellationToken);

    Task<IReadOnlyList<Enrollment>> GetEnrollmentsForUserAsync(Guid userId, CancellationToken cancellationToken);

    Task AddCertificateAsync(Certificate certificate, CancellationToken cancellationToken);

    Task<Certificate?> GetCertificateAsync(Guid userId, Guid courseId, CancellationToken cancellationToken);
}
