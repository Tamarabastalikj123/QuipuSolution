using MediatR;

namespace QuipuSolution.Application.Features.Books.Commands.DeleteBook
{
    /// <summary>Deletes a book. Throws NotFoundException when it does not exist.</summary>
    public record DeleteBookCommand(int BookId) : IRequest;
}
