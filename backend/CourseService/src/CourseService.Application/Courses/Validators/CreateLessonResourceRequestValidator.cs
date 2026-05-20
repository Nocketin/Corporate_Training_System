using CourseService.Application.Courses.Dtos;
using FluentValidation;

namespace CourseService.Application.Courses.Validators;

public class CreateLessonResourceRequestValidator : AbstractValidator<CreateLessonResourceRequest>
{
    public CreateLessonResourceRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Kind).NotEmpty();

        When(x => x.Kind.Equals("link", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.Url)
                .NotEmpty()
                .Must(BeValidUri).WithMessage("Resource URL must be a valid absolute URI.");
        });

        When(x => x.Kind.Equals("file", StringComparison.OrdinalIgnoreCase) && x.Id is null, () =>
        {
            RuleFor(x => x.FileKey).NotEmpty();
        });
    }

    private static bool BeValidUri(string? url) =>
        !string.IsNullOrWhiteSpace(url) && Uri.TryCreate(url, UriKind.Absolute, out _);
}
