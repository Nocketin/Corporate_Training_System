using LearningService.Application.Dtos;
using MediatR;

namespace LearningService.Application.Analytics.Queries;

public record GetLearningAnalyticsOverviewQuery(DateOnly? From, DateOnly? To) : IRequest<LearningAnalyticsOverviewDto>;
