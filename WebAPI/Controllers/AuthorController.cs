using Business.Abstract;
using Core.Utilities.Pagination;
using Entities.DTOs.AuthorDTOS;
using Entities.DTOs.UserDTOs;
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

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllAuthorsAsync([FromQuery] PaginationParameters pagination)
        {
            var authors = await _authorService.GetAllAsync(pagination);
            return Ok(authors);
        }
        [HttpGet("getById")]
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
            var result = await _authorService.AddAsync(entity);
            return StatusCode((int)result.StatusCode, result);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(Guid id, UpdateAuthorDTO entity)
        {
           var result = await _authorService.UpdateAsync(id, entity);
           return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var result = await _authorService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO entity)
        {
            var result = await _authorService.RegisterAsync(entity);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO entity)
        {
            var result = await _authorService.LoginAsync(entity);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
