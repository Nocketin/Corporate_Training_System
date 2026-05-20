using LearningService.Application.Abstractions;
using LearningService.Application.Analytics.Queries;
using LearningService.Application.Dtos;
using MediatR;

namespace LearningService.Application.Analytics.Handlers;

public class GetLearningAnalyticsOverviewQueryHandler
    : IRequestHandler<GetLearningAnalyticsOverviewQuery, LearningAnalyticsOverviewDto>
{
    private readonly ILearningAnalyticsReadRepository _repository;

    public GetLearningAnalyticsOverviewQueryHandler(ILearningAnalyticsReadRepository repository)
    {
        _repository = repository;
    }

    public Task<LearningAnalyticsOverviewDto> Handle(
        GetLearningAnalyticsOverviewQuery request,
        CancellationToken cancellationToken)
    {
        var to = request.To ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var from = request.From ?? to.AddDays(-30);

        var fromUtc = DateTime.SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var toUtcExclusive = DateTime.SpecifyKind(to.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        return _repository.GetOverviewAsync(fromUtc, toUtcExclusive, cancellationToken);
    }
}
