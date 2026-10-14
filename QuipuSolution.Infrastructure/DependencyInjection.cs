using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuipuSolution.Application.Contracts.Repository.Authors;
using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Infrastructure.Persistence;
using QuipuSolution.Infrastructure.Repositories;

namespace QuipuSolution.Infrastructure
{
    public static class DependencyInjection
    {
        /// <summary>Tag of the health checks that must pass before the app can serve requests.</summary>
        public const string ReadinessTag = "ready";

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            services.AddDbContext<QuipuSolutionDbContext>(options =>
                options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();

            services.AddHealthChecks()
                .AddDbContextCheck<QuipuSolutionDbContext>("database", tags: [ReadinessTag]);

            return services;
        }

        /// <summary>
        /// Applies pending EF Core migrations (creates the database if it does not exist).
        /// </summary>
        public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<QuipuSolutionDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}
