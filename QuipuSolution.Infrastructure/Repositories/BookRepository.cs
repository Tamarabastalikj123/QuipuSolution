using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Domain.Entities;
using QuipuSolution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace QuipuSolution.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly QuipuSolutionDbContext _context;

        public BookRepository(QuipuSolutionDbContext context)
        {
            _context = context;
        }
        public async Task<List<Book>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .OrderBy(b => b.BookId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Book?> GetByIdAsync(int bookId, CancellationToken cancellationToken = default)
        {
            return await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.BookId == bookId, cancellationToken);
        }

        public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
        {
            await _context.Books.AddAsync(book, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Book book, CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Book book, CancellationToken cancellationToken = default)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<(List<Book> Items, int TotalCount)> SearchAsync(
            string? title,
            string? author,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                var titleFilter = title.Trim();
                query = query.Where(b => b.Title.Contains(titleFilter));
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                var authorFilter = author.Trim();
                query = query.Where(b => b.Author.Name.Contains(authorFilter));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(b => b.BookId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
