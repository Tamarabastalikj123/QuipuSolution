using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.IntegrationTests.Infrastructure;

namespace QuipuSolution.IntegrationTests
{
    public class BookCrudTests : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory _factory;
        private readonly HttpClient _client;

        private static CancellationToken Ct => TestContext.Current.CancellationToken;

        public BookCrudTests(ApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateCrudClient();
        }

        [Fact]
        public async Task GetAll_WithoutToken_Returns401()
        {
            var response = await _factory.CreateClient().GetAsync("/api/book", Ct);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_WithSearchToken_Returns403()
        {
            var response = await _factory.CreateSearchClient().GetAsync("/api/book", Ct);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_ReturnsSeededBooksWithAuthors()
        {
            var books = await _client.GetFromJsonAsync<List<BookDto>>("/api/book", Ct);

            var book = Assert.Single(books!, b => b.BookId == 1);
            Assert.Equal("Crime and Punishment", book.Title);
            Assert.Equal("Fyodor Dostoevsky", book.Author.Name);
        }

        [Fact]
        public async Task GetById_MissingBook_Returns404ProblemDetails()
        {
            var response = await _client.GetAsync("/api/book/9999", Ct);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(Ct);
            Assert.Equal("Book with id 9999 was not found.", problem!.Detail);
        }

        [Fact]
        public async Task Create_ValidBook_Returns201AndCanBeRead()
        {
            var response = await _client.PostAsJsonAsync("/api/book",
                new CreateBookRequest { Title = "The Idiot", SubTitle = "A novel", AuthorId = 1 }, Ct);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<BookDto>(Ct);
            Assert.NotNull(response.Headers.Location);

            var loaded = await _client.GetFromJsonAsync<BookDto>(response.Headers.Location, Ct);
            Assert.Equal(created!.BookId, loaded!.BookId);
            Assert.Equal("The Idiot", loaded.Title);
            Assert.Equal("A novel", loaded.SubTitle);
            Assert.Equal(1, loaded.Author.AuthorId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("ab")]
        public async Task Create_InvalidTitle_Returns400(string title)
        {
            var response = await _client.PostAsJsonAsync("/api/book",
                new CreateBookRequest { Title = title, AuthorId = 1 }, Ct);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(Ct);
            Assert.Contains("Title", problem!.Errors.Keys);
        }

        [Fact]
        public async Task Create_TitleTooLong_Returns400()
        {
            var response = await _client.PostAsJsonAsync("/api/book",
                new CreateBookRequest { Title = new string('a', 101), AuthorId = 1 }, Ct);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_UnknownAuthor_Returns404()
        {
            var response = await _client.PostAsJsonAsync("/api/book",
                new CreateBookRequest { Title = "Valid title", AuthorId = 999 }, Ct);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Update_ExistingBook_Returns200WithNewValues()
        {
            var id = await CreateBookAsync("Old title");

            var response = await _client.PutAsJsonAsync($"/api/book/{id}",
                new UpdateBookRequest { Title = "Emma", SubTitle = "Updated", AuthorId = 2 }, Ct);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updated = await response.Content.ReadFromJsonAsync<BookDto>(Ct);
            Assert.Equal("Emma", updated!.Title);
            Assert.Equal("Updated", updated.SubTitle);
            Assert.Equal(2, updated.Author.AuthorId);
        }

        [Fact]
        public async Task Update_MissingBook_Returns404()
        {
            var response = await _client.PutAsJsonAsync("/api/book/9999",
                new UpdateBookRequest { Title = "Emma", AuthorId = 2 }, Ct);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ExistingBook_Returns204AndBookIsGone()
        {
            var id = await CreateBookAsync("To be deleted");

            var deleteResponse = await _client.DeleteAsync($"/api/book/{id}", Ct);
            var getResponse = await _client.GetAsync($"/api/book/{id}", Ct);

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        private async Task<int> CreateBookAsync(string title)
        {
            var response = await _client.PostAsJsonAsync("/api/book",
                new CreateBookRequest { Title = title, AuthorId = 1 }, Ct);
            response.EnsureSuccessStatusCode();
            var book = await response.Content.ReadFromJsonAsync<BookDto>(Ct);
            return book!.BookId;
        }
    }
}
