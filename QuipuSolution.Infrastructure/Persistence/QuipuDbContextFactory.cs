using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace QuipuSolution.Infrastructure.Persistence
{
    public class QuipuDbContextFactory : IDesignTimeDbContextFactory<QuipuSolutionDbContext>
    {
      //  public QuipuSolutionDbContext CreateDbContext(string[] args)
      //  {
            //var options = new DbContextOptionsBuilder<QuipuSolutionDbContext>()
            //    .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BookstoreDb;Trusted_Connection=True;TrustServerCertificate=True")
            //    .Options;

            //return new QuipuSolutionDbContext(options);

            //var configuration = new ConfigurationBuilder()
            // .SetBasePath(Directory.GetCurrentDirectory())
            // .AddJsonFile("appsettings.json")
            // .Build();

            //var optionsBuilder = new DbContextOptionsBuilder<QuipuSolutionDbContext>();
            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            //optionsBuilder.UseSqlServer(connectionString)
            //             //tak .LogTo(Console.WriteLine, LogLevel.Information)  // Log SQL to console
            //              .EnableSensitiveDataLogging();  // Log detailed information

            //optionsBuilder.UseSqlServer(connectionString);

            //return new QuipuSolutionDbContext(optionsBuilder.Options);

              public QuipuSolutionDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<QuipuSolutionDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new QuipuSolutionDbContext(options);
        }


    }
}
