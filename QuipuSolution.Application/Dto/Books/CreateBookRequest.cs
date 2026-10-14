using System.ComponentModel.DataAnnotations;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Application.Dto.Books
{
    public class CreateBookRequest
    {
        [Required]
        [StringLength(Book.TitleMaxLength, MinimumLength = Book.TitleMinLength)]
        public string Title { get; set; } = string.Empty;

        public string? SubTitle { get; set; }

        [Range(1, int.MaxValue)]
        public int AuthorId { get; set; }
    }
}
