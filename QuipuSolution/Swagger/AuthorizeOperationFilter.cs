using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using QuipuSolution.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QuipuSolution.Swagger
{
    /// <summary>
    /// Attaches the matching OAuth2 flow to each endpoint, based on its authorization policy,
    /// so Swagger UI sends the right token (client credentials or implicit) for each call.
    /// </summary>
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public const string ClientCredentialsScheme = "client_credentials";
        public const string ImplicitScheme = "implicit";

        private readonly string _crudScope;
        private readonly string _searchScope;

        public AuthorizeOperationFilter(string crudScope, string searchScope)
        {
            _crudScope = crudScope;
            _searchScope = searchScope;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var policies = context.MethodInfo.GetCustomAttributes<AuthorizeAttribute>(true)
                .Concat(context.MethodInfo.DeclaringType?.GetCustomAttributes<AuthorizeAttribute>(true) ?? [])
                .Select(attribute => attribute.Policy)
                .Distinct();

            foreach (var policy in policies)
            {
                var (scheme, scope) = policy switch
                {
                    AuthorizationPolicies.BookCrud => (ClientCredentialsScheme, _crudScope),
                    AuthorizationPolicies.BookSearch => (ImplicitScheme, _searchScope),
                    _ => (null, null)
                };

                if (scheme is null)
                    continue;

                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = scheme }
                        },
                        new[] { scope }
                    }
                });

                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Missing or invalid access token" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Access token does not grant access to this endpoint" });
            }
        }
    }
}
