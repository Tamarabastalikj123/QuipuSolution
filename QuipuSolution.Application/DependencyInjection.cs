using Microsoft.Extensions.DependencyInjection;
using QuipuSolution.Application.Contracts.Services;
using QuipuSolution.Application.Services.Books;

namespace QuipuSolution.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();

            return services;
        }
    }
}
