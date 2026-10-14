using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Application.Contracts.Repository.Books
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Book?> GetByIdAsync(int bookId, CancellationToken cancellationToken = default);
        Task AddAsync(Book book, CancellationToken cancellationToken = default);
        Task UpdateAsync(Book book, CancellationToken cancellationToken = default);
        Task DeleteAsync(Book book, CancellationToken cancellationToken = default);
        Task<(List<Book> Items, int TotalCount)> SearchAsync(
            string? title,
            string? author,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
