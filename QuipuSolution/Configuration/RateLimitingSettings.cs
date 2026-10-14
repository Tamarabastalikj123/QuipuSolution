using System.ComponentModel.DataAnnotations;

namespace QuipuSolution.Configuration
{
    /// <summary>
    /// The "RateLimiting" section of appsettings.json.
    /// </summary>
    public class RateLimitingSettings
    {
        public const string SectionName = "RateLimiting";

        /// <summary>Maximum number of requests per caller within one window.</summary>
        [Range(1, int.MaxValue)]
        public int PermitLimit { get; set; } = 100;

        /// <summary>Length of the window in seconds.</summary>
        [Range(1, int.MaxValue)]
        public int WindowSeconds { get; set; } = 60;
    }
}
