using System.ComponentModel.DataAnnotations;

namespace QuipuSolution.IdentityServer
{
    /// <summary>
    /// The "AuthServer" section of appsettings.json. Secrets and URLs come from configuration
    /// </summary>
    public class AuthServerSettings
    {
        public const string SectionName = "AuthServer";

        [Required]
        public string ClientCredentialsClientId { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string ClientCredentialsClientSecret { get; set; } = string.Empty;

        /// <summary>Client id of the browser client (implicit flow).</summary>
        [Required]
        public string ImplicitClientId { get; set; } = string.Empty;

        /// <summary>Origins of the Swagger UI allowed to request tokens.</summary>
        [Required, MinLength(1)]
        public string[] SwaggerOrigins { get; set; } = [];

        /// <summary>Enables the demo users (alice, bob). Must be off in production - use a real user store instead.</summary>
        public bool EnableTestUsers { get; set; }

        /// <summary>
        /// Fixed issuer (iss claim). Needed when the server is reached under different addresses,
        /// When empty, the issuer is taken from the request URL.
        /// </summary>
        [Url]
        public string? IssuerUri { get; set; }

        /// <summary>
        /// Folder for the Data Protection keys (they protect cookies and the token signing keys).
        /// Must be persistent so keys survive restarts. 
        /// When empty, the framework default is used.
        /// </summary>
        public string? DataProtectionKeysPath { get; set; }

        public static AuthServerSettings Load(IConfiguration configuration)
        {
            var settings = configuration.GetSection(SectionName).Get<AuthServerSettings>()
                ?? throw new InvalidOperationException($"Configuration section '{SectionName}' is missing.");

            Validator.ValidateObject(settings, new ValidationContext(settings), validateAllProperties: true);

            return settings;
        }
    }
}
