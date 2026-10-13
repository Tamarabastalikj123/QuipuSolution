namespace QuipuSolution.Application.Dto.Books
{
    public record UpdateBookRequest(string Title, string? SubTitle, int AuthorId);
}
