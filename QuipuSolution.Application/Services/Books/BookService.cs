using QuipuSolution.Application.Contracts.Repository.Authors;
using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Application.Contracts.Services;
using QuipuSolution.Application.Dto.Authors;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Dto.Common;
using QuipuSolution.Application.Exceptions;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Application.Services.Books
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _books;
        private readonly IAuthorRepository _authors;

        public BookService(IBookRepository books, IAuthorRepository authors)
        {
            _books = books;
            _authors = authors;
        }

        public async Task<List<BookDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var books = await _books.GetAllAsync(cancellationToken);
            return books.Select(ToDto).ToList();
        }

        public async Task<BookDto> GetByIdAsync(int bookId, CancellationToken cancellationToken = default)
        {
            var book = await GetBookOrThrow(bookId, cancellationToken);
            return ToDto(book);
        }

        public async Task<PagedResult<BookDto>> SearchAsync(BookSearchRequest request, CancellationToken cancellationToken = default)
        {
            var (books, totalCount) = await _books.SearchAsync(
                request.Title,
                request.Author,
                request.Page,
                request.PageSize,
                cancellationToken);

            return new PagedResult<BookDto>
            {
                Items = books.Select(ToDto).ToList(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
        {
            var author = await GetAuthorOrThrow(request.AuthorId, cancellationToken);

            var book = new Book(request.Title, request.SubTitle, author);
            await _books.AddAsync(book, cancellationToken);

            return ToDto(book);
        }

        public async Task<BookDto> UpdateAsync(int bookId, UpdateBookRequest request, CancellationToken cancellationToken = default)
        {
            var book = await GetBookOrThrow(bookId, cancellationToken);
            var author = await GetAuthorOrThrow(request.AuthorId, cancellationToken);

            book.SetTitle(request.Title);
            book.SetSubTitle(request.SubTitle);
            book.SetAuthor(author);

            await _books.UpdateAsync(book, cancellationToken);

            return ToDto(book);
        }

        public async Task DeleteAsync(int bookId, CancellationToken cancellationToken = default)
        {
            var book = await GetBookOrThrow(bookId, cancellationToken);
            await _books.DeleteAsync(book, cancellationToken);
        }

        private async Task<Book> GetBookOrThrow(int bookId, CancellationToken cancellationToken) =>
            await _books.GetByIdAsync(bookId, cancellationToken)
            ?? throw new NotFoundException($"Book with id {bookId} was not found.");

        private async Task<Author> GetAuthorOrThrow(int authorId, CancellationToken cancellationToken) =>
            await _authors.GetByIdAsync(authorId, cancellationToken)
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
