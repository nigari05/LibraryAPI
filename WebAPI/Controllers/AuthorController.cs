using Business.Abstract;
using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.AuthorDTOS;
using Entities.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Bütün müəllifləri səhifələnmiş şəkildə qaytarır.
        /// </summary>
        /// <param name="pagination">Səhifə nömrəsi, ölçüsü və sıralama parametrləri.</param>
        /// <response code="200">Müəllif siyahısı uğurla qaytarıldı.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IDataResult<List<GetAuthorDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAuthorsAsync([FromQuery] PaginationParameters pagination)
        {
            var authors = await _authorService.GetAllAsync(pagination);
            return Ok(authors);
        }
        /// <summary>
        /// ID-yə görə tək bir müəllifi qaytarır.
        /// </summary>
        /// <param name="id">Müəllifin unikal identifikatoru.</param>
        /// <response code="200">Müəllif tapıldı.</response>
        /// <response code="404">Bu ID ilə müəllif tapılmadı.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IDataResult<GetAuthorDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null)
                return NotFound();
            return Ok(author);
        }
        /// <summary>
        /// Yeni müəllif əlavə edir. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <param name="entity">Yaradılacaq müəllifin məlumatları.</param>
        /// <response code="201">Müəllif uğurla yaradıldı.</response>
        /// <response code="400">Göndərilən məlumatlar keçərsiz idi.</response>
        /// <response code="401">İstifadəçi autentifikasiya olunmayıb.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<IActionResult> CreateAuthor(CreateAuthorDTO entity)
        {
            var result = await _authorService.AddAsync(entity);
            return StatusCode((int)result.StatusCode, result);

        }
        /// <summary>
        /// Mövcud müəllifin məlumatlarını yeniləyir. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <param name="id">Yenilənəcək müəllifin identifikatoru.</param>
        /// <param name="entity">Yeni müəllif məlumatları.</param>
        /// <response code="204">Müəllif uğurla yeniləndi.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        /// <response code="404">Bu ID ilə müəllif tapılmadı.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateAuthor(Guid id, UpdateAuthorDTO entity)
        {
           var result = await _authorService.UpdateAsync(id, entity);
           return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Müəllifi sistemdən silir. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <param name="id">Silinəcək müəllifin identifikatoru.</param>
        /// <response code="204">Müəllif uğurla silindi.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        /// <response code="404">Bu ID ilə müəllif tapılmadı.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var result = await _authorService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

       
    }
}
