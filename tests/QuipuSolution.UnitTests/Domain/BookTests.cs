using QuipuSolution.Domain.Common;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.UnitTests.Domain
{
    public class BookTests
    {
        private static readonly Author Author = new("Fyodor Dostoevsky");

        [Fact]
        public void Constructor_WithValidValues_CreatesBook()
        {
            var book = new Book("Crime and Punishment", "A novel", Author);

            Assert.Equal("Crime and Punishment", book.Title);
            Assert.Equal("A novel", book.SubTitle);
            Assert.Same(Author, book.Author);
        }

        [Fact]
        public void Constructor_TrimsTitleAndSubTitle()
        {
            var book = new Book("  The Idiot  ", "  A novel  ", Author);

            Assert.Equal("The Idiot", book.Title);
            Assert.Equal("A novel", book.SubTitle);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ab")]
        [InlineData("  ab  ")]
        public void SetTitle_TooShort_Throws(string title)
        {
            Assert.Throws<DomainException>(() => new Book(title, null, Author));
        }

        [Fact]
        public void SetTitle_TooLong_Throws()
        {
            var title = new string('a', Book.TitleMaxLength + 1);

            Assert.Throws<DomainException>(() => new Book(title, null, Author));
        }

        [Fact]
        public void SetTitle_AtLengthLimits_IsAccepted()
        {
            var shortest = new Book(new string('a', Book.TitleMinLength), null, Author);
            var longest = new Book(new string('a', Book.TitleMaxLength), null, Author);

            Assert.Equal(Book.TitleMinLength, shortest.Title.Length);
            Assert.Equal(Book.TitleMaxLength, longest.Title.Length);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void SetSubTitle_Empty_StoresNull(string? subTitle)
        {
            var book = new Book("Crime and Punishment", subTitle, Author);

            Assert.Null(book.SubTitle);
        }

        [Fact]
        public void SetAuthor_Null_Throws()
        {
            Assert.Throws<DomainException>(() => new Book("Crime and Punishment", null, null!));
        }
    }
}
