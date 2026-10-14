using QuipuSolution.Domain.Common;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.UnitTests.Domain
{
    public class AuthorTests
    {
        [Fact]
        public void Constructor_WithValidName_TrimsAndStoresName()
        {
            var author = new Author("  Jane Austen  ");

            Assert.Equal("Jane Austen", author.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("ab")]
        [InlineData("   ")]
        public void SetName_TooShort_Throws(string name)
        {
            Assert.Throws<DomainException>(() => new Author(name));
        }

        [Fact]
        public void SetName_TooLong_Throws()
        {
            var name = new string('a', Author.NameMaxLength + 1);

            Assert.Throws<DomainException>(() => new Author(name));
        }
    }
}
