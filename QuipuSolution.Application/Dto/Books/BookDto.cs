using QuipuSolution.Application.Dto.Authors;

namespace QuipuSolution.Application.Dto.Books
{
    public class BookDto
    {
        public int BookId { get; set; }
        public AuthorDto Author { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? SubTitle { get; set; }
    }
}
