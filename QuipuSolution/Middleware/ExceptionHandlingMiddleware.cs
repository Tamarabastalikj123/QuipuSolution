using Microsoft.AspNetCore.Mvc;
using QuipuSolution.Application.Exceptions;
using QuipuSolution.Domain.Common;

namespace QuipuSolution.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var (statusCode, title) = exception switch
                {
                    NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                    DomainException => (StatusCodes.Status400BadRequest, "Bad Request"),
                    _ => (StatusCodes.Status500InternalServerError, "Server Error")
                };

                if (statusCode == StatusCodes.Status500InternalServerError)
                    _logger.LogError(exception, "Unhandled exception");

                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = statusCode == StatusCodes.Status500InternalServerError
                        ? "An unexpected error occurred."
                        : exception.Message
                };

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
