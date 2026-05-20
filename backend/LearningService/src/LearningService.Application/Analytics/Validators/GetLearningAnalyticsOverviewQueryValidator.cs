using FluentValidation;
using LearningService.Application.Analytics.Queries;

namespace LearningService.Application.Analytics.Validators;

public class GetLearningAnalyticsOverviewQueryValidator : AbstractValidator<GetLearningAnalyticsOverviewQuery>
{
    public GetLearningAnalyticsOverviewQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => !x.From.HasValue || !x.To.HasValue || x.From <= x.To)
            .WithMessage("From date must be before or equal to To date.");

        RuleFor(x => x)
            .Must(x =>
            {
                if (!x.From.HasValue || !x.To.HasValue)
                {
                    return true;
                }

                return x.To.Value.DayNumber - x.From.Value.DayNumber <= 365;
            })
            .WithMessage("Date range cannot exceed 365 days.");
    }
}
