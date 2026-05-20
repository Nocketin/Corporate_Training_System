using FluentValidation;
using LearningService.Application.Learning.Commands;

namespace LearningService.Application.Learning.Validators;

public class EnrollInCourseCommandValidator : AbstractValidator<EnrollInCourseCommand>
{
    public EnrollInCourseCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
