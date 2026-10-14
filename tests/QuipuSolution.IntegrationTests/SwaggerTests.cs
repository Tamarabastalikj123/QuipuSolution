using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using QuipuSolution.IntegrationTests.Infrastructure;

namespace QuipuSolution.IntegrationTests
{
    public class SwaggerApiFactory : ApiFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseSetting("Swagger:Enabled", "true");
        }
    }

    public class SwaggerTests : IClassFixture<SwaggerApiFactory>
    {
        private readonly HttpClient _client;

        private static CancellationToken Ct => TestContext.Current.CancellationToken;

        public SwaggerTests(SwaggerApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Document_DefinesBothOAuthFlows()
        {
            using var document = await GetDocumentAsync();

            var schemes = document.RootElement.GetProperty("components").GetProperty("securitySchemes");
            Assert.True(schemes.GetProperty("client_credentials").GetProperty("flows").TryGetProperty("clientCredentials", out _));
            Assert.True(schemes.GetProperty("implicit").GetProperty("flows").TryGetProperty("implicit", out _));
        }

        [Theory]
        [InlineData("/api/book", "get", "client_credentials")]
        [InlineData("/api/book", "post", "client_credentials")]
        [InlineData("/api/book/{bookId}", "put", "client_credentials")]
        [InlineData("/api/book/{bookId}", "delete", "client_credentials")]
        [InlineData("/api/book/search", "get", "implicit")]
        public async Task EachEndpoint_UsesTheFlowOfItsPolicy(string path, string method, string expectedScheme)
        {
            using var document = await GetDocumentAsync();

            var operation = document.RootElement.GetProperty("paths").GetProperty(path).GetProperty(method);
            var schemes = operation.GetProperty("security").EnumerateArray()
                .SelectMany(requirement => requirement.EnumerateObject().Select(p => p.Name));

            Assert.Equal([expectedScheme], schemes);
        }

        private async Task<JsonDocument> GetDocumentAsync()
        {
            var json = await _client.GetStringAsync("/swagger/v1/swagger.json", Ct);
            return JsonDocument.Parse(json);
        }
    }
}
