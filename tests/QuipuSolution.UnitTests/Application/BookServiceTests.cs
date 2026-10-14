using NSubstitute;
using QuipuSolution.Application.Contracts.Repository.Authors;
using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Exceptions;
using QuipuSolution.Application.Services.Books;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.UnitTests.Application
{
    public class BookServiceTests
    {
        private readonly IBookRepository _books = Substitute.For<IBookRepository>();
        private readonly IAuthorRepository _authors = Substitute.For<IAuthorRepository>();
        private readonly BookService _service;

        private readonly Author _dostoevsky = new("Fyodor Dostoevsky");
        private readonly Author _austen = new("Jane Austen");

        private static CancellationToken Ct => TestContext.Current.CancellationToken;

        public BookServiceTests()
        {
            _service = new BookService(_books, _authors);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingBook_ReturnsMappedDto()
        {
            _books.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new Book("Crime and Punishment", "A novel", _dostoevsky));

            var result = await _service.GetByIdAsync(1, Ct);

            Assert.Equal("Crime and Punishment", result.Title);
            Assert.Equal("A novel", result.SubTitle);
            Assert.Equal("Fyodor Dostoevsky", result.Author.Name);
        }

        [Fact]
        public async Task GetByIdAsync_MissingBook_ThrowsNotFound()
        {
            _books.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Book?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99, Ct));
        }

        [Fact]
        public async Task CreateAsync_ExistingAuthor_AddsBook()
        {
            _authors.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(_dostoevsky);
            var request = new CreateBookRequest { Title = "The Idiot", SubTitle = null, AuthorId = 1 };

            var result = await _service.CreateAsync(request, Ct);

            Assert.Equal("The Idiot", result.Title);
            Assert.Equal("Fyodor Dostoevsky", result.Author.Name);
            await _books.Received(1).AddAsync(Arg.Is<Book>(b => b.Title == "The Idiot"), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreateAsync_UnknownAuthor_ThrowsNotFound_AndDoesNotSave()
        {
            _authors.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Author?)null);
            var request = new CreateBookRequest { Title = "The Idiot", AuthorId = 99 };

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(request, Ct));
            await _books.DidNotReceiveWithAnyArgs().AddAsync(default!, Ct);
        }

        [Fact]
        public async Task UpdateAsync_ExistingBook_ChangesAllFields()
        {
            var book = new Book("Old title", "Old subtitle", _dostoevsky);
            _books.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(book);
            _authors.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(_austen);
            var request = new UpdateBookRequest { Title = "Emma", SubTitle = null, AuthorId = 2 };

            var result = await _service.UpdateAsync(1, request, Ct);

            Assert.Equal("Emma", result.Title);
            Assert.Null(result.SubTitle);
            Assert.Equal("Jane Austen", result.Author.Name);
            await _books.Received(1).UpdateAsync(book, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_MissingBook_ThrowsNotFound()
        {
            _books.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Book?)null);
            var request = new UpdateBookRequest { Title = "Emma", AuthorId = 2 };

            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(99, request, Ct));
        }

        [Fact]
        public async Task DeleteAsync_ExistingBook_DeletesIt()
        {
            var book = new Book("Crime and Punishment", null, _dostoevsky);
            _books.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(book);

            await _service.DeleteAsync(1, Ct);

            await _books.Received(1).DeleteAsync(book, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAsync_MissingBook_ThrowsNotFound()
        {
            _books.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Book?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(99, Ct));
            await _books.DidNotReceiveWithAnyArgs().DeleteAsync(default!, Ct);
        }

        [Fact]
        public async Task SearchAsync_PassesFiltersAndReturnsPagingInfo()
        {
            _books.SearchAsync("crime", "dosto", 2, 1, Arg.Any<CancellationToken>())
                .Returns((new List<Book> { new("Crime and Punishment", null, _dostoevsky) }, 3));
            var request = new BookSearchRequest { Title = "crime", Author = "dosto", Page = 2, PageSize = 1 };

            var result = await _service.SearchAsync(request, Ct);

            Assert.Single(result.Items);
            Assert.Equal(2, result.Page);
            Assert.Equal(1, result.PageSize);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.TotalPages);
        }
    }
}
