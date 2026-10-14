using System.Net;
using QuipuSolution.IntegrationTests.Infrastructure;

namespace QuipuSolution.IntegrationTests
{
    public class HealthAndSecurityTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;

        private static CancellationToken Ct => TestContext.Current.CancellationToken;

        public HealthAndSecurityTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("/health/live")]
        [InlineData("/health/ready")]
        public async Task HealthEndpoints_AreAnonymousAndHealthy(string url)
        {
            var response = await _client.GetAsync(url, Ct);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Healthy", await response.Content.ReadAsStringAsync(Ct));
        }

        [Fact]
        public async Task Responses_IncludeSecurityHeaders()
        {
            var response = await _client.GetAsync("/health/live", Ct);

            Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
            Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
            Assert.True(response.Headers.Contains("Content-Security-Policy"));
        }

        [Fact]
        public async Task Swagger_IsDisabledOutsideDevelopment()
        {
            var response = await _client.GetAsync("/swagger/v1/swagger.json", Ct);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
