using System.Net;
using System.Net.Http.Json;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Dto.Common;
using QuipuSolution.IntegrationTests.Infrastructure;

namespace QuipuSolution.IntegrationTests
{
    /// <summary>
    /// Runs against the 4 seeded books. This class never modifies data, so the counts are stable.
    /// Note: SQLite compares case-sensitively, SQL Server (default collation) does not - so these tests use matching case.
    /// </summary>
    public class BookSearchTests : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory _factory;
        private readonly HttpClient _client;

        private static CancellationToken Ct => TestContext.Current.CancellationToken;

        public BookSearchTests(ApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateSearchClient();
        }

        [Fact]
        public async Task Search_WithoutToken_Returns401()
        {
            var response = await _factory.CreateClient().GetAsync("/api/book/search", Ct);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithClientCredentialsToken_Returns403()
        {
            var response = await _factory.CreateCrudClient().GetAsync("/api/book/search", Ct);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithSearchScopeButNoUser_Returns403()
        {
            var client = _factory.CreateClientWith(ApiFactory.SearchScope, subject: null);

            var response = await client.GetAsync("/api/book/search", Ct);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithoutFilters_ReturnsAllBooksPaged()
        {
            var result = await SearchAsync("");

            Assert.Equal(4, result.TotalCount);
            Assert.Equal(4, result.Items.Count);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(1, result.TotalPages);
        }

        [Fact]
        public async Task Search_ByPartialAuthorName_ReturnsMatchingBooks()
        {
            var result = await SearchAsync("?author=Dosto");

            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Items, book => Assert.Equal("Fyodor Dostoevsky", book.Author.Name));
        }

        [Fact]
        public async Task Search_ByPartialTitle_ReturnsMatchingBooks()
        {
            var result = await SearchAsync("?title=The");

            Assert.Equal(3, result.TotalCount);
            Assert.All(result.Items, book => Assert.Contains("The", book.Title));
        }

        [Fact]
        public async Task Search_ByTitleAndAuthor_RequiresBothToMatch()
        {
            var result = await SearchAsync("?title=Crime&author=Dosto");

            var book = Assert.Single(result.Items);
            Assert.Equal("Crime and Punishment", book.Title);
        }

        [Fact]
        public async Task Search_NoMatch_ReturnsEmptyPage()
        {
            var result = await SearchAsync("?title=Nonexistent");

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(0, result.TotalPages);
        }

        [Fact]
        public async Task Search_Pagination_ReturnsRequestedPageInStableOrder()
        {
            var page1 = await SearchAsync("?pageSize=2&page=1");
            var page2 = await SearchAsync("?pageSize=2&page=2");
            var page3 = await SearchAsync("?pageSize=2&page=3");

            Assert.Equal([1, 2], page1.Items.Select(b => b.BookId));
            Assert.Equal([3, 4], page2.Items.Select(b => b.BookId));
            Assert.Empty(page3.Items);
            Assert.Equal(2, page2.TotalPages);
            Assert.Equal(4, page2.TotalCount);
        }

        [Theory]
        [InlineData("?page=0")]
        [InlineData("?pageSize=0")]
        [InlineData("?pageSize=51")]
        public async Task Search_InvalidPaging_Returns400(string query)
        {
            var response = await _client.GetAsync($"/api/book/search{query}", Ct);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private async Task<PagedResult<BookDto>> SearchAsync(string query)
        {
            var result = await _client.GetFromJsonAsync<PagedResult<BookDto>>($"/api/book/search{query}", Ct);
            return result!;
        }
    }
}
