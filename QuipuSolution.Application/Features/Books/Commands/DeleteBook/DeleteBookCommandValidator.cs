using FluentValidation;
using QuipuSolution.Application.Features.Books.Validation;

namespace QuipuSolution.Application.Features.Books.Commands.DeleteBook
{
    public class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
    {
        public DeleteBookCommandValidator()
        {
            RuleFor(command => command.BookId).ValidId();
        }
    }
}
