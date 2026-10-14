using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuipuSolution.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "AuthorId", "SubTitle", "Title" },
                values: new object[,]
                {
                    { 1, 1, null, "Crime and Punishment" },
                    { 2, 1, null, "The Brothers Karamazov" },
                    { 3, 3, null, "The Housemaid" },
                    { 4, 4, "A Robert Hunter Thriller", "The Crucifix Killer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 4);
        }
    }
}
