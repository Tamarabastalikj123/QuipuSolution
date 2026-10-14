using QuipuSolution.Domain.Common;

namespace QuipuSolution.Domain.Entities
{
    public class Author
    {
        public const int NameMinLength = 3;
        public const int NameMaxLength = 100;

        public int AuthorId { get; private set; }
        public string Name { get; private set; } = null!;

        private Author() { } // EF Core

        public Author(string name)
        {
            SetName(name);
        }

        public void SetName(string name)
        {
            name = name?.Trim() ?? string.Empty;
            if (name.Length < NameMinLength || name.Length > NameMaxLength)
                throw new DomainException(
                    $"Author name must be between {NameMinLength} and {NameMaxLength} characters.");
            Name = name;
        }
    }
}
