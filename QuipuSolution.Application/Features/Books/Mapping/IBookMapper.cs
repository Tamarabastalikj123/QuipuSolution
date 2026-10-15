using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Features.Books.Commands.CreateBook;
using QuipuSolution.Application.Features.Books.Commands.UpdateBook;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Application.Features.Books.Mapping
{
    /// <summary>
    /// Maps between API requests, CQRS commands, domain entities and response DTOs.
    /// </summary>
    public interface IBookMapper
    {
        BookDto ToDto(Book book);

        List<BookDto> ToDtos(IEnumerable<Book> books);

        CreateBookCommand ToCommand(CreateBookRequest request);

        UpdateBookCommand ToCommand(int bookId, UpdateBookRequest request);
    }
}
