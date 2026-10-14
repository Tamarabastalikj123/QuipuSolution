using QuipuSolution.Application.Dto.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuipuSolution.Application.Contracts.Services
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllAsync(CancellationToken cancellation = default);
        Task<BookDto> GetByIdAsync(int bookId, CancellationToken cancellation = default);
        Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellation = default);
        Task<BookDto> UpdateAsync(int bookId, UpdateBookRequest request, CancellationToken cancellation = default);
        Task DeleteAsync(int bookId, CancellationToken cancellation = default);
    }
}
