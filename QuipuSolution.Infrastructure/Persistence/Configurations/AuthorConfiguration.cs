using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Infrastructure.Persistence.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors");

            builder.HasKey(a => a.AuthorId);
            builder.Property(a => a.AuthorId).ValueGeneratedOnAdd();

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(Author.NameMaxLength);

            // The API only manages books, so authors are seeded
            builder.HasData(
                new { AuthorId = 1, Name = "Fyodor Dostoevsky" },
                new { AuthorId = 2, Name = "Katerina Shoshko" },
                new { AuthorId = 3, Name = "Frida Mcfadden" },
                new { AuthorId = 4, Name = "Chris Carter" });
        }
    }
}
