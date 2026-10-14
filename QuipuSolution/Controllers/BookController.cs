using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuipuSolution.Application.Contracts.Services;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Authorization;

namespace QuipuSolution.Controllers
{
    /// <summary>
    /// CRUD operations for books. Requires a token from the client credentials flow (scope: book.api).
    /// </summary>
    [ApiController]
    [Route("api/book")]
    [Produces("application/json")]
    [Authorize(Policy = AuthorizationPolicies.BookCrud)]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Returns all books.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<BookDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BookDto>>> GetAll(CancellationToken cancellationToken)
        {
            var books = await _bookService.GetAllAsync(cancellationToken);
            return Ok(books);
        }

        /// <summary>
        /// Returns a single book.
        /// </summary>
        /// <param name="bookId">Id of the book.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{bookId:int}")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> GetById(int bookId, CancellationToken cancellationToken)
        {
            var book = await _bookService.GetByIdAsync(bookId, cancellationToken);
            return Ok(book);
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <remarks>The author must already exist (see the seeded authors).</remarks>
        [HttpPost]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> Create(CreateBookRequest request, CancellationToken cancellationToken)
        {
            var book = await _bookService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { bookId = book.BookId }, book);
        }

        /// <summary>
        /// Updates an existing book.
        /// </summary>
        /// <param name="bookId">Id of the book.</param>
        /// <param name="request">New values of the book.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{bookId:int}")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> Update(int bookId, UpdateBookRequest request, CancellationToken cancellationToken)
        {
            var book = await _bookService.UpdateAsync(bookId, request, cancellationToken);
            return Ok(book);
        }

        /// <summary>
        /// Deletes a book.
        /// </summary>
        /// <param name="bookId">Id of the book.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{bookId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int bookId, CancellationToken cancellationToken)
        {
            await _bookService.DeleteAsync(bookId, cancellationToken);
            return NoContent();
        }
    }
}
