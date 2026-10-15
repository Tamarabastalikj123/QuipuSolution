using MediatR;
using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Application.Exceptions;

namespace QuipuSolution.Application.Features.Books.Commands.DeleteBook
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
    {
        private readonly IBookRepository _books;

        public DeleteBookCommandHandler(IBookRepository books)
        {
            _books = books;
        }

        public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _books.GetByIdAsync(request.BookId, cancellationToken)
                ?? throw new NotFoundException($"Book with id {request.BookId} was not found.");

            await _books.DeleteAsync(book, cancellationToken);
        }
    }
}
