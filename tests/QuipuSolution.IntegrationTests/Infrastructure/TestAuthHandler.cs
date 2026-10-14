using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace QuipuSolution.IntegrationTests.Infrastructure
{
    /// <summary>
    /// Replaces JWT validation in tests: the caller's scopes and user id come from request headers.
    /// The API's real authorization policies still decide whether the request is allowed.
    /// </summary>
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Test";
        public const string ScopeHeader = "X-Test-Scope";
        public const string SubjectHeader = "X-Test-Sub";

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ScopeHeader, out var scopes))
                return Task.FromResult(AuthenticateResult.NoResult());

            var claims = scopes.ToString()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(scope => new Claim("scope", scope))
                .ToList();

            if (Request.Headers.TryGetValue(SubjectHeader, out var subject))
                claims.Add(new Claim("sub", subject.ToString()));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
        }
    }
}
