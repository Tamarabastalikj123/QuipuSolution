using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Application.Contracts.Repository.Authors
{
    public interface IAuthorRepository
    {
        Task<Author?> GetByIdAsync(int authorId, CancellationToken cancellationToken = default);
    }
}
