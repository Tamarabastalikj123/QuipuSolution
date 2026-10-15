using MediatR;
using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Features.Books.Mapping;

namespace QuipuSolution.Application.Features.Books.Queries.GetBooks
{
    public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, List<BookDto>>
    {
        private readonly IBookRepository _books;
        private readonly IBookMapper _mapper;

        public GetBooksQueryHandler(IBookRepository books, IBookMapper mapper)
        {
            _books = books;
            _mapper = mapper;
        }

        public async Task<List<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
        {
            var books = await _books.GetAllAsync(cancellationToken);
            return _mapper.ToDtos(books);
        }
    }
}
