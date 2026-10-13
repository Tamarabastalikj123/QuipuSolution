using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Infrastructure.Configurations
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
        }
    }
}
