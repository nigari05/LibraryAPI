using Business.Abstract;
using Core.Utilities.Pagination;
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
        public async Task<IActionResult> GetAllBooks([FromQuery]PaginationParameters pagination)
        {
            var result = await _bookService.GetAllBooksAsync(pagination);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet("getbookbyid/{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var result = await _bookService.GetByIdAsync(id);
           
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost("addbook")]
        public async Task<IActionResult> CreateBook(CreateBookDTO entity)
        {
            var result = await _bookService.AddAsync(entity);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPut("updatebook/{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, UpdateBookDTO entity)
        {
            var result = await _bookService.UpdateAsync(id, entity);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("deletebook/{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var result = await _bookService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
