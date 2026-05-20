using CourseService.Application.Courses.Commands;
using FluentValidation;

namespace CourseService.Application.Courses.Validators;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);

        When(x => x.Modules is { Count: > 0 }, () =>
        {
            RuleForEach(x => x.Modules).ChildRules(m =>
            {
                m.RuleFor(x => x!.Title).NotEmpty();
                m.RuleFor(x => x!.Order).GreaterThanOrEqualTo(0);
                m.RuleFor(x => x!.Lessons).NotEmpty();
                m.RuleForEach(x => x!.Lessons).ChildRules(l =>
                {
                    l.RuleFor(y => y!.Title).NotEmpty();
                    l.RuleFor(y => y!.Order).GreaterThanOrEqualTo(0);
                    l.RuleFor(y => y!.DurationMinutes).GreaterThanOrEqualTo(0);
                    l.RuleForEach(y => y!.Resources).SetValidator(new CreateLessonResourceRequestValidator());
                });
            });
        });
    }
}
