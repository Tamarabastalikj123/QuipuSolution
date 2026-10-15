using FluentValidation;
using QuipuSolution.Application.Features.Books.Validation;

namespace QuipuSolution.Application.Features.Books.Commands.CreateBook
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
            RuleFor(command => command.Title).ValidBookTitle();
            RuleFor(command => command.AuthorId).ValidId();
        }
    }
}
