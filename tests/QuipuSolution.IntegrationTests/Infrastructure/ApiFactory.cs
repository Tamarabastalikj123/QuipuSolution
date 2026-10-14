using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using QuipuSolution.Infrastructure.Persistence;

namespace QuipuSolution.IntegrationTests.Infrastructure
{
    /// <summary>
    /// Runs the real API in memory with an in-memory SQLite database (seeded with the same data as SQL Server)
    /// and header-based test authentication. Each test class gets its own instance, so its own database.
    /// </summary>
    public class ApiFactory : WebApplicationFactory<Program>
    {
        public const string CrudScope = "book.api";
        public const string SearchScope = "book.search";

        // The in-memory database lives as long as this connection is open
        private readonly SqliteConnection _connection = new("DataSource=:memory:");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _connection.Open();

            builder.UseEnvironment("Testing");
            builder.UseSetting("ConnectionStrings:DefaultConnection", "replaced-by-sqlite");
            builder.UseSetting("IdentityServer:Authority", "https://identityserver.test");
            builder.UseSetting("RateLimiting:PermitLimit", "10000");

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<QuipuSolutionDbContext>>();
                services.AddDbContext<QuipuSolutionDbContext>(options => options.UseSqlite(_connection));

                services.AddAuthentication(TestAuthHandler.SchemeName)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<QuipuSolutionDbContext>().Database.EnsureCreated();

            return host;
        }

        /// <summary>Client with a token as issued by the client credentials flow.</summary>
        public HttpClient CreateCrudClient() => CreateClientWith(CrudScope, subject: null);

        /// <summary>Client with a token as issued by the implicit flow to a logged-in user.</summary>
        public HttpClient CreateSearchClient() => CreateClientWith(SearchScope, subject: "1");

        public HttpClient CreateClientWith(string scope, string? subject)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Add(TestAuthHandler.ScopeHeader, scope);
            if (subject is not null)
                client.DefaultRequestHeaders.Add(TestAuthHandler.SubjectHeader, subject);
            return client;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                _connection.Dispose();
        }
    }
}
