using Business.Abstract;
using Entities.DTOs.AuthorDTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAsync();
            return Ok(authors);
        }
        [HttpGet]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null)
                return NotFound();
            return Ok(author);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAuthor(CreateAuthorDTO entity)
        {
            await _authorService.AddAsync(entity);
            return Ok("Author added successfully.");

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(Guid id, UpdateAuthorDTO entity)
        {
            await _authorService.UpdateAsync(id, entity);
            return Ok("Author updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            await _authorService.DeleteAsync(id);
            return Ok("Author deleted successfully.");
        }
    }
}
