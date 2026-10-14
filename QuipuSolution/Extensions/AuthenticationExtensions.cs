using Microsoft.AspNetCore.Authentication.JwtBearer;
using QuipuSolution.Authorization;
using QuipuSolution.Configuration;

namespace QuipuSolution.Extensions
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Validates JWT access tokens issued by IdentityServer and registers one policy per protected area.
        /// </summary>
        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IdentityServerSettings settings)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = settings.Authority;
                    options.Audience = settings.Audience;
                    options.RequireHttpsMetadata = settings.RequireHttpsMetadata;

                    // Keep the original JWT claim names ("sub", "scope") instead of mapping them to long URIs
                    options.MapInboundClaims = false;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.BookCrud, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", settings.CrudScope);
                });

                options.AddPolicy(AuthorizationPolicies.BookSearch, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", settings.SearchScope);
                    policy.RequireClaim("sub"); // token must belong to a logged-in user
                });
            });

            return services;
        }
    }
}
