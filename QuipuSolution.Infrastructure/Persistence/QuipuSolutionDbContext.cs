
using Microsoft.EntityFrameworkCore;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Infrastructure.Persistence
{
    public class QuipuSolutionDbContext :DbContext
    {
        public QuipuSolutionDbContext(DbContextOptions<QuipuSolutionDbContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuipuSolutionDbContext).Assembly);
        }
    }
}
