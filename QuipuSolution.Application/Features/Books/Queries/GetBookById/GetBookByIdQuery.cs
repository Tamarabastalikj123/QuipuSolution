using MediatR;
using QuipuSolution.Application.Dto.Books;

namespace QuipuSolution.Application.Features.Books.Queries.GetBookById
{
    /// <summary>Returns one book. Throws NotFoundException when it does not exist.</summary>
    public record GetBookByIdQuery(int BookId) : IRequest<BookDto>;
}
