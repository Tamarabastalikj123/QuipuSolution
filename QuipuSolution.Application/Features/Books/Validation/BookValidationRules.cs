using FluentValidation;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Application.Features.Books.Validation
{
    /// <summary>
    /// Rules shared by several book validators, based on the limits defined in the domain.
    /// </summary>
    public static class BookValidationRules
    {
        public static IRuleBuilderOptions<T, string> ValidBookTitle<T>(this IRuleBuilder<T, string> rule) =>
            rule
                .NotEmpty()
                // Empty titles are reported by NotEmpty; this rule checks the length of the trimmed title (as stored by the domain)
                .Must(title => string.IsNullOrWhiteSpace(title)
                    || title.Trim().Length is >= Book.TitleMinLength and <= Book.TitleMaxLength)
                .WithMessage($"Title must be between {Book.TitleMinLength} and {Book.TitleMaxLength} characters.");

        public static IRuleBuilderOptions<T, int> ValidId<T>(this IRuleBuilder<T, int> rule) =>
            rule.GreaterThan(0);
    }
}
