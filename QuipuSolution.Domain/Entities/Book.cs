using QuipuSolution.Domain.Common;

namespace QuipuSolution.Domain.Entities
{
    public class Book
    {
        public const int TitleMinLength = 3;
        public const int TitleMaxLength = 100;

        public int BookId { get; private set; }
        public string Title { get; private set; } = null!;
        public string? SubTitle { get; private set; }

        public int AuthorId { get; private set; }
        public Author Author { get; private set; } = null!;

        private Book() { } // EF Core

        public Book(string title, string? subTitle, Author author)
        {
            SetTitle(title);
            SetSubTitle(subTitle);
            SetAuthor(author);
        }

        public void SetTitle(string title)
        {
            title = title?.Trim() ?? string.Empty;
            if (title.Length < TitleMinLength || title.Length > TitleMaxLength)
                throw new DomainException(
                    $"Book title must be between {TitleMinLength} and {TitleMaxLength} characters.");
            Title = title;
        }

        public void SetSubTitle(string? subTitle)
        {
            SubTitle = string.IsNullOrWhiteSpace(subTitle) ? null : subTitle.Trim();
        }

        public void SetAuthor(Author author)
        {
            Author = author ?? throw new DomainException("Book must have an author.");
            AuthorId = author.AuthorId;
        }
    }
}
