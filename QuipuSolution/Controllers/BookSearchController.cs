using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuipuSolution.Application.Contracts.Services;
using QuipuSolution.Application.Dto.Books;
using QuipuSolution.Application.Dto.Common;
using QuipuSolution.Authorization;

namespace QuipuSolution.Controllers
{
    /// <summary>
    /// Book search. Requires a token from the implicit flow, issued to a logged-in user (scope: book.search).
    /// </summary>
    [ApiController]
    [Route("api/book/search")]
    [Produces("application/json")]
    [Authorize(Policy = AuthorizationPolicies.BookSearch)]
    public class BookSearchController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookSearchController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Searches books by title and/or author name, with pagination.
        /// </summary>
        /// <remarks>
        /// Both filters are optional, case-insensitive and match any part of the text.
        /// When both are given, a book must match both.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<BookDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<BookDto>>> Search(
            [FromQuery] BookSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _bookService.SearchAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}
