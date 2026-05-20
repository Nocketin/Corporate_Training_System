using FluentValidation.Results;

namespace IdentityService.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("Validation failed")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
