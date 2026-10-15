using MediatR;
using QuipuSolution.Application.Contracts.Repository.Authors;
using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Exceptions;
using QuipuSolution.Application.Features.Books.Mapping;
using QuipuSolution.Domain.Entities;

namespace QuipuSolution.Application.Features.Books.Commands.CreateBook
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
    {
        private readonly IBookRepository _books;
        private readonly IAuthorRepository _authors;
        private readonly IBookMapper _mapper;

        public CreateBookCommandHandler(IBookRepository books, IAuthorRepository authors, IBookMapper mapper)
        {
            _books = books;
            _authors = authors;
            _mapper = mapper;
        }

        public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var author = await _authors.GetByIdAsync(request.AuthorId, cancellationToken)
                ?? throw new NotFoundException($"Author with id {request.AuthorId} was not found.");

            var book = new Book(request.Title, request.SubTitle, author);
            await _books.AddAsync(book, cancellationToken);

            return _mapper.ToDto(book);
        }
    }
}
