using Duende.IdentityServer.Models;

namespace QuipuSolution.IdentityServer
{
    public static class Config
    {
        public const string BooksApiScope = "book.api";
        public const string BooksApiResource = "book-api";

        public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope(BooksApiScope, "Book API")
        ];

        public static IEnumerable<ApiResource> ApiResources =>
        [
            new ApiResource(BooksApiResource, "Book API")
            {
                Scopes = { BooksApiScope }
            }
        ];

        public static IEnumerable<Client> Clients =>
        [
            new Client
            {
                ClientId = "quipu.client",
                ClientName = "Quipu Book Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("quipu-secret".Sha256()) },
                AllowedScopes = { BooksApiScope },
                AllowedCorsOrigins = { "https://localhost:7107" }
            }
        ];
    }
}

