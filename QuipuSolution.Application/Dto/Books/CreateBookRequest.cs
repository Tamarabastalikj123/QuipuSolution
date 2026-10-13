namespace QuipuSolution.Application.Dto.Books
{
    public record CreateBookRequest(string Title, string? SubTitle, int AuthorId);
}
