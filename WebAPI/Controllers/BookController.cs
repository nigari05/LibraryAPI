using Business.Abstract;
using Entities.DTOs.BookDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("getallbooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            var result = await _bookService.GetAllBooksAsync();
            return Ok(result);
        }
        [HttpGet("getbookbyid/{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var result = await _bookService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [HttpPost("addbook")]
        public async Task<IActionResult> CreateBook(CreateBookDTO entity)
        {
            await _bookService.AddAsync(entity);
            return Ok("Book added successfully.");
        }
        [HttpPut("updatebook/{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, UpdateBookDTO entity)
        {
            await _bookService.UpdateAsync(id, entity);
            return Ok("Book updated successfully.");
        }

        [HttpDelete("deletebook/{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _bookService.DeleteAsync(id);
            return Ok("Book deleted successfully.");
        }
    }
}
