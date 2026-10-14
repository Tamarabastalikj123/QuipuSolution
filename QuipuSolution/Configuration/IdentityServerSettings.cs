using System.ComponentModel.DataAnnotations;

namespace QuipuSolution.Configuration
{
    /// <summary>
    /// The "IdentityServer" section of appsettings.json.
    /// </summary>
    public class IdentityServerSettings
    {
        public const string SectionName = "IdentityServer";

        /// <summary>Base URL of the token server, as reachable from the API (used to download the signing keys).</summary>
        [Required, Url]
        public string Authority { get; set; } = string.Empty;

        /// <summary>
        /// Base URL of the token server, as reachable from the browser (used by Swagger UI).
        /// Only needed when it differs from Authority, e.g. in Docker.
        /// </summary>
        [Url]
        public string? PublicAuthority { get; set; }

        /// <summary>Require HTTPS for the token server metadata. Disable only for local HTTP setups such as Docker Compose.</summary>
        public bool RequireHttpsMetadata { get; set; } = true;

        public string BrowserAuthority => string.IsNullOrWhiteSpace(PublicAuthority) ? Authority : PublicAuthority;

        /// <summary>The API resource name the tokens must be issued for (aud claim).</summary>
        [Required]
        public string Audience { get; set; } = string.Empty;

        /// <summary>Scope required by the CRUD endpoints (client credentials flow).</summary>
        [Required]
        public string CrudScope { get; set; } = string.Empty;

        /// <summary>Scope required by the search endpoint (implicit flow).</summary>
        [Required]
        public string SearchScope { get; set; } = string.Empty;
    }

    public static class IdentityServerSettingsExtensions
    {
        /// <summary>
        /// Reads and validates the IdentityServer settings - fails at startup if anything is missing.
        /// </summary>
        public static IdentityServerSettings GetIdentityServerSettings(this IConfiguration configuration)
        {
            var settings = configuration.GetSection(IdentityServerSettings.SectionName).Get<IdentityServerSettings>()
                ?? throw new InvalidOperationException($"Configuration section '{IdentityServerSettings.SectionName}' is missing.");

            Validator.ValidateObject(settings, new ValidationContext(settings), validateAllProperties: true);

            return settings;
        }
    }
}
