using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Features.Books.Commands.DeleteBook;
using QuipuSolution.Application.Features.Books.Mapping;
using QuipuSolution.Application.Features.Books.Queries.GetBookById;
using QuipuSolution.Application.Features.Books.Queries.GetBooks;

namespace QuipuSolution.Controllers
{
    [ApiController]
    [Route("api/book")]
    [Authorize(Policy = "BookApiScope")]
    public class BookController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IBookMapper _mapper;

        public BookController(ISender sender, IBookMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetAll(CancellationToken cancellationToken)
        {
            var books = await _sender.Send(new GetBooksQuery(), cancellationToken);
            return Ok(books);
        }

        [HttpGet("{bookId:int}")]
        public async Task<ActionResult<BookDto>> GetById(int bookId, CancellationToken cancellationToken)
        {
            var book = await _sender.Send(new GetBookByIdQuery(bookId), cancellationToken);
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create(CreateBookRequest request, CancellationToken cancellationToken)
        {
            var book = await _sender.Send(_mapper.ToCommand(request), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { bookId = book.BookId }, book);
        }

        [HttpPut("{bookId:int}")]
        public async Task<ActionResult<BookDto>> Update(int bookId, UpdateBookRequest request, CancellationToken cancellationToken)
        {
            var book = await _sender.Send(_mapper.ToCommand(bookId, request), cancellationToken);
            return Ok(book);
        }

        [HttpDelete("{bookId:int}")]
        public async Task<IActionResult> Delete(int bookId, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteBookCommand(bookId), cancellationToken);
            return NoContent();
        }
    }
}
