using MediatR;
using QuipuSolution.Application.Dto.Books;

namespace QuipuSolution.Application.Features.Books.Commands.CreateBook
{
    /// <summary>Creates a book for an existing author and returns it.</summary>
    public record CreateBookCommand(string Title, string? SubTitle, int AuthorId) : IRequest<BookDto>;
}
