using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Dto.Common;

namespace QuipuSolution.Application.Contracts.Services
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<BookDto> GetByIdAsync(int bookId, CancellationToken cancellationToken = default);
        Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
        Task<BookDto> UpdateAsync(int bookId, UpdateBookRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int bookId, CancellationToken cancellationToken = default);
        Task<PagedResult<BookDto>> SearchAsync(BookSearchRequest request, CancellationToken cancellationToken = default);
    }
}
