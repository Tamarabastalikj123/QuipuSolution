namespace QuipuSolution.Extensions
{
    public static class SecurityHeadersExtensions
    {
        /// <summary>
        /// Adds standard security headers to every response.
        /// The API only returns JSON, so the content security policy blocks everything (except for the Swagger UI pages).
        /// </summary>
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;
                headers.XContentTypeOptions = "nosniff";
                headers.XFrameOptions = "DENY";
                headers["Referrer-Policy"] = "no-referrer";

                if (!context.Request.Path.StartsWithSegments("/swagger"))
                    headers.ContentSecurityPolicy = "default-src 'none'; frame-ancestors 'none'";

                await next();
            });
        }
    }
}
