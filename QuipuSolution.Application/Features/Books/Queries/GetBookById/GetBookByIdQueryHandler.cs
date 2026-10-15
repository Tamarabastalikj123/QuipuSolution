using MediatR;
using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Exceptions;
using QuipuSolution.Application.Features.Books.Mapping;

namespace QuipuSolution.Application.Features.Books.Queries.GetBookById
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto>
    {
        private readonly IBookRepository _books;
        private readonly IBookMapper _mapper;

        public GetBookByIdQueryHandler(IBookRepository books, IBookMapper mapper)
        {
            _books = books;
            _mapper = mapper;
        }

        public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _books.GetByIdAsync(request.BookId, cancellationToken)
                ?? throw new NotFoundException($"Book with id {request.BookId} was not found.");

            return _mapper.ToDto(book);
        }
    }
}
