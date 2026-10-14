using System.ComponentModel.DataAnnotations;

namespace QuipuSolution.Application.Dto.Books
{
    public class BookSearchRequest
    {
        public string? Title { get; set; }

        public string? Author { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
