using QuipuSolution.Application.Contracts.Repository.Authors;
using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Application.Contracts.Services;
using QuipuSolution.Application.Dto.Authors;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Exceptions;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Application.Service.Books
{
    public class BookService :IBookService
    {
        private readonly IBookRepository _books;
        private readonly IAuthorRepository _authors;

        public BookService(IBookRepository books, IAuthorRepository authors)
        {
            _books = books;
            _authors = authors;
        }

        public async Task<List<BookDto>> GetAllAsync(CancellationToken ct = default)
        {
            var books = await _books.GetAllAsync(ct);
            return books.Select(ToDto).ToList();
        }

        public async Task<BookDto> GetByIdAsync(int bookId, CancellationToken ct = default)
        {
            var book = await GetBookOrThrow(bookId, ct);
            return ToDto(book);
        }

        public async Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
        {
            var author = await GetAuthorOrThrow(request.AuthorId, ct);

            var book = new Book(request.Title, request.SubTitle, author);
            await _books.AddAsync(book, ct);

            return ToDto(book);
        }

        public async Task<BookDto> UpdateAsync(int bookId, UpdateBookRequest request, CancellationToken ct = default)
        {
            var book = await GetBookOrThrow(bookId, ct);
            var author = await GetAuthorOrThrow(request.AuthorId, ct);

            book.SetTitle(request.Title);
            book.SetSubTitle(request.SubTitle);
            book.SetAuthor(author);

            await _books.UpdateAsync(book, ct);

            return ToDto(book);
        }

        public async Task DeleteAsync(int bookId, CancellationToken ct = default)
        {
            var book = await GetBookOrThrow(bookId, ct);
            await _books.DeleteAsync(book, ct);
        }

        private async Task<Book> GetBookOrThrow(int bookId, CancellationToken ct) =>
            await _books.GetByIdAsync(bookId, ct)
            ?? throw new NotFoundException($"Book with id {bookId} was not found.");

        private async Task<Author> GetAuthorOrThrow(int authorId, CancellationToken ct) =>
            await _authors.GetByIdAsync(authorId, ct)
            ?? throw new NotFoundException($"Author with id {authorId} was not found.");

        private static BookDto ToDto(Book book) => new()
        {
            BookId = book.BookId,
            Title = book.Title,
            SubTitle = book.SubTitle,
            Author = new AuthorDto
            {
                AuthorId = book.Author.AuthorId,
                Name = book.Author.Name
            }
        };
    }
}
