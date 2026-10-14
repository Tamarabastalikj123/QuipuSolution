using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace QuipuSolution.Infrastructure.Persistence
{
    /// <summary>
    /// Used only by the EF Core tools (dotnet ef) to create the DbContext at design time.
    /// Reads the connection string from the startup project's settings (Development by default) or environment variables.
    /// </summary>
    public class QuipuDbContextFactory : IDesignTimeDbContextFactory<QuipuSolutionDbContext>
    {
        public QuipuSolutionDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<QuipuSolutionDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new QuipuSolutionDbContext(options);
        }
    }
}
