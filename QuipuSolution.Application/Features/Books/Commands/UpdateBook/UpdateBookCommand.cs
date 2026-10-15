using MediatR;
using QuipuSolution.Application.Dto.Books;

namespace QuipuSolution.Application.Features.Books.Commands.UpdateBook
{
    /// <summary>Replaces the title, subtitle and author of an existing book and returns it.</summary>
    public record UpdateBookCommand(int BookId, string Title, string? SubTitle, int AuthorId) : IRequest<BookDto>;
}
