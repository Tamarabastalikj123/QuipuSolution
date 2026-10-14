using Microsoft.AspNetCore.Mvc;
using QuipuSolution.Application.Contracts.Services;
using QuipuSolution.Application.Dto.Books;

namespace QuipuSolution.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetAll(CancellationToken cancellationToken)
        {
            var books = await _bookService.GetAllAsync(cancellationToken);
            return Ok(books);
        }

        [HttpGet("{bookId:int}")]
        public async Task<ActionResult<BookDto>> GetById(int bookId, CancellationToken cancellationToken)
        {
            var book = await _bookService.GetByIdAsync(bookId, cancellationToken);
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create(CreateBookRequest request, CancellationToken cancellationToken)
        {
            var book = await _bookService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { bookId = book.BookId }, book);
        }

        [HttpPut("{bookId:int}")]
        public async Task<ActionResult<BookDto>> Update(int bookId, UpdateBookRequest request, CancellationToken cancellationToken)
        {
            var book = await _bookService.UpdateAsync(bookId, request, cancellationToken);
            return Ok(book);
        }

        [HttpDelete("{bookId:int}")]
        public async Task<IActionResult> Delete(int bookId, CancellationToken cancellationToken)
        {
            await _bookService.DeleteAsync(bookId, cancellationToken);
            return NoContent();
        }
    }
}
