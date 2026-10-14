using System.Reflection;
using Microsoft.OpenApi.Models;
using QuipuSolution.Configuration;
using QuipuSolution.Swagger;

namespace QuipuSolution.Extensions
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Swagger document with one OAuth2 flow per protected area and the XML comments of the controllers.
        /// </summary>
        public static IServiceCollection AddApiSwagger(this IServiceCollection services, IdentityServerSettings settings)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Quipu Books API",
                    Version = "v1",
                    Description = "CRUD for books (OAuth2 client credentials) and book search with pagination (OAuth2 implicit flow)."
                });

                options.AddSecurityDefinition(AuthorizeOperationFilter.ClientCredentialsScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "CRUD endpoints - client credentials flow",
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{settings.BrowserAuthority}/connect/token"),
                            Scopes = new Dictionary<string, string> { [settings.CrudScope] = "Create, read, update and delete books" }
                        }
                    }
                });

                options.AddSecurityDefinition(AuthorizeOperationFilter.ImplicitScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "Search endpoint - implicit flow (user login)",
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{settings.BrowserAuthority}/connect/authorize"),
                            Scopes = new Dictionary<string, string> { [settings.SearchScope] = "Search books" }
                        }
                    }
                });

                options.OperationFilter<AuthorizeOperationFilter>(settings.CrudScope, settings.SearchScope);

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            });

            return services;
        }

        public static WebApplication UseApiSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = "Quipu Books API";
            });

            return app;
        }
    }
}
