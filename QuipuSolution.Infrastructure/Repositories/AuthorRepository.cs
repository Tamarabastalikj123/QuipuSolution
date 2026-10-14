using Microsoft.EntityFrameworkCore;
using QuipuSolution.Application.Contracts.Repository.Authors;
using QuipuSolution.Domain.Entities;
using QuipuSolution.Infrastructure.Persistence;

namespace QuipuSolution.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly QuipuSolutionDbContext _context;

        public AuthorRepository(QuipuSolutionDbContext context)
        {
            _context = context;
        }

        public async Task<Author?> GetByIdAsync(int authorId, CancellationToken cancellationToken = default)
        {
            return await _context.Authors
                .FirstOrDefaultAsync(a => a.AuthorId == authorId, cancellationToken);
        }
    }
}
