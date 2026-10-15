using MediatR;
using QuipuSolution.Application.Dto.Books;

namespace QuipuSolution.Application.Features.Books.Queries.GetBooks
{
    /// <summary>Returns all books.</summary>
    public record GetBooksQuery : IRequest<List<BookDto>>;
}
