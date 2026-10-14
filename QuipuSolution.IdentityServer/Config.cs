using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;

namespace QuipuSolution.IdentityServer
{
    public static class Config
    {
        public const string BooksApiResource = "book-api";

        // Scope for the CRUD endpoints (client credentials flow)
        public const string BooksApiScope = "book.api";

        // Scope for the search endpoint (implicit flow)
        public const string BooksSearchScope = "book.search";

        public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope(BooksApiScope, "Book API - CRUD"),
            new ApiScope(BooksSearchScope, "Book API - Search")
        ];

        public static IEnumerable<ApiResource> ApiResources =>
        [
            new ApiResource(BooksApiResource, "Book API")
            {
                Scopes = { BooksApiScope, BooksSearchScope }
            }
        ];

        public static IEnumerable<Client> GetClients(AuthServerSettings settings) =>
        [
            // Machine-to-machine client: CRUD endpoints
            new Client
            {
                ClientId = settings.ClientCredentialsClientId,
                ClientName = "Quipu Book Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret(settings.ClientCredentialsClientSecret.Sha256()) },
                AllowedScopes = { BooksApiScope },
                AllowedCorsOrigins = settings.SwaggerOrigins
            },

            // Browser client (Swagger UI): search endpoint, user logs in
            new Client
            {
                ClientId = settings.ImplicitClientId,
                ClientName = "Quipu Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                RequireClientSecret = false,
                AllowAccessTokensViaBrowser = true,
                RequireConsent = false,
                RedirectUris = settings.SwaggerOrigins.Select(origin => $"{origin}/swagger/oauth2-redirect.html").ToList(),
                AllowedCorsOrigins = settings.SwaggerOrigins,
                AllowedScopes = { BooksSearchScope }
            }
        ];

        public static List<TestUser> GetUsers(AuthServerSettings settings) =>
            settings.EnableTestUsers ? TestUsers : [];

        // Demo users only - a real deployment plugs in a proper user store (e.g. ASP.NET Core Identity)
        private static List<TestUser> TestUsers =>
        [
            new TestUser
            {
                SubjectId = "1",
                Username = "alice",
                Password = "alice",
                Claims = { new Claim("name", "Alice Smith") }
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "bob",
                Password = "bob",
                Claims = { new Claim("name", "Bob Smith") }
            }
        ];
    }
}
