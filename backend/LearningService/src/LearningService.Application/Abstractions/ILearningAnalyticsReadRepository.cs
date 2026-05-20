using LearningService.Application.Dtos;

namespace LearningService.Application.Abstractions;

public interface ILearningAnalyticsReadRepository
{
    Task<LearningAnalyticsOverviewDto> GetOverviewAsync(
        DateTime fromUtc,
        DateTime toUtcExclusive,
        CancellationToken cancellationToken);
}
