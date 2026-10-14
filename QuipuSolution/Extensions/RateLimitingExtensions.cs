using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.RateLimiting;
using QuipuSolution.Configuration;

namespace QuipuSolution.Extensions
{
    public static class RateLimitingExtensions
    {
        public const string ApiPolicy = "api";

        /// <summary>
        /// Fixed-window rate limit per caller: the logged-in user (sub), else the client (client_id), else the IP address.
        /// </summary>
        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(RateLimitingSettings.SectionName).Get<RateLimitingSettings>() ?? new RateLimitingSettings();
            Validator.ValidateObject(settings, new ValidationContext(settings), validateAllProperties: true);

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(ApiPolicy, httpContext =>
                {
                    var partitionKey = httpContext.User.FindFirst("sub")?.Value
                        ?? httpContext.User.FindFirst("client_id")?.Value
                        ?? httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                        QueueLimit = 0
                    });
                });

                options.OnRejected = async (context, _) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                    }

                    var problemDetails = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                    await problemDetails.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context.HttpContext,
                        ProblemDetails =
                        {
                            Status = StatusCodes.Status429TooManyRequests,
                            Title = "Too Many Requests",
                            Detail = "Rate limit exceeded. Try again later."
                        }
                    });
                };
            });

            return services;
        }
    }
}
