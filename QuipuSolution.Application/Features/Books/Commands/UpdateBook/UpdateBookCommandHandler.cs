using MediatR;
using QuipuSolution.Application.Contracts.Repository.Authors;
using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Exceptions;
using QuipuSolution.Application.Features.Books.Mapping;

namespace QuipuSolution.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
    {
        private readonly IBookRepository _books;
        private readonly IAuthorRepository _authors;
        private readonly IBookMapper _mapper;

        public UpdateBookCommandHandler(IBookRepository books, IAuthorRepository authors, IBookMapper mapper)
        {
            _books = books;
            _authors = authors;
            _mapper = mapper;
        }

        public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _books.GetByIdAsync(request.BookId, cancellationToken)
                ?? throw new NotFoundException($"Book with id {request.BookId} was not found.");

            var author = await _authors.GetByIdAsync(request.AuthorId, cancellationToken)
                ?? throw new NotFoundException($"Author with id {request.AuthorId} was not found.");

            book.SetTitle(request.Title);
            book.SetSubTitle(request.SubTitle);
            book.SetAuthor(author);

            await _books.UpdateAsync(book, cancellationToken);

            return _mapper.ToDto(book);
        }
    }
}
