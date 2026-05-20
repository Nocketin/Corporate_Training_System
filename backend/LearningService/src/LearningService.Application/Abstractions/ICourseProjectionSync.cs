namespace LearningService.Application.Abstractions;

public interface ICourseProjectionSync
{
    Task SyncFromCatalogAsync(Guid courseId, CancellationToken cancellationToken);
}
