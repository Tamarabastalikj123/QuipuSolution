using FluentValidation;
using QuipuSolution.Application.Features.Books.Validation;

namespace QuipuSolution.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator()
        {
            RuleFor(command => command.BookId).ValidId();
            RuleFor(command => command.Title).ValidBookTitle();
            RuleFor(command => command.AuthorId).ValidId();
        }
    }
}
