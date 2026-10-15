using QuipuSolution.Application.Dto.Authors;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Features.Books.Commands.CreateBook;
using QuipuSolution.Application.Features.Books.Commands.UpdateBook;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Application.Features.Books.Mapping
{
    /// <summary>
    /// Explicit, hand-written mapping: every field is visible, checked by the compiler and easy to unit test.
    /// </summary>
    public class BookMapper : IBookMapper
    {
        public BookDto ToDto(Book book) => new()
        {
            BookId = book.BookId,
            Title = book.Title,
            SubTitle = book.SubTitle,
            Author = ToDto(book.Author)
        };

        public List<BookDto> ToDtos(IEnumerable<Book> books) => books.Select(ToDto).ToList();

        public CreateBookCommand ToCommand(CreateBookRequest request) =>
            new(request.Title, request.SubTitle, request.AuthorId);

        public UpdateBookCommand ToCommand(int bookId, UpdateBookRequest request) =>
            new(bookId, request.Title, request.SubTitle, request.AuthorId);

        private static AuthorDto ToDto(Author author) => new()
        {
            AuthorId = author.AuthorId,
            Name = author.Name
        };
    }
}
