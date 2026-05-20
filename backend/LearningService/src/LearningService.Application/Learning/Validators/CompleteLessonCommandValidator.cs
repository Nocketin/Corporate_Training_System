using FluentValidation;
using LearningService.Application.Learning.Commands;

namespace LearningService.Application.Learning.Validators;

public class CompleteLessonCommandValidator : AbstractValidator<CompleteLessonCommand>
{
    public CompleteLessonCommandValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
