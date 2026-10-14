using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Infrastructure.Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.BookId);
            builder.Property(b => b.BookId).ValueGeneratedOnAdd();

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(Book.TitleMaxLength);

            builder.Property(b => b.SubTitle);

            builder.HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new { BookId = 1, Title = "Crime and Punishment", SubTitle = (string?)null, AuthorId = 1 },
                new { BookId = 2, Title = "The Brothers Karamazov", SubTitle = (string?)null, AuthorId = 1 },
                new { BookId = 3, Title = "The Housemaid", SubTitle = (string?)null, AuthorId = 3 },
                new { BookId = 4, Title = "The Crucifix Killer", SubTitle = "A Robert Hunter Thriller", AuthorId = 4 });
        }
    }
}
