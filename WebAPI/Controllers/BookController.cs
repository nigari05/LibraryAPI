using Business.Abstract;
using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.BookDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Bütün kitabları səhifələnmiş şəkildə qaytarır.
        /// </summary>
        /// <param name="pagination">Səhifə nömrəsi, ölçüsü və sıralama parametrləri.</param>
        /// <response code="200">Kitab siyahısı uğurla qaytarıldı.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IDataResult<PagedResult<GetBookDTO>>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetAllBooks([FromQuery]PaginationParameters pagination)
        {
            var result = await _bookService.GetAllBooksAsync(pagination);
            return StatusCode((int)result.StatusCode, result);
        }
        /// <summary>
        /// ID-yə görə tək bir kitabı qaytarır.
        /// </summary>
        /// <param name="id">Kitabın unikal identifikatoru.</param>
        /// <response code="200">Kitab tapıldı.</response>
        /// <response code="404">Bu ID ilə kitab tapılmadı.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IDataResult<GetBookDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var result = await _bookService.GetByIdAsync(id);
           
            return StatusCode((int)result.StatusCode, result);
        }
        /// <summary>
        /// Yeni kitab əlavə edir. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <param name="entity">Yaradılacaq kitabın məlumatları.</param>
        /// <response code="201">Kitab uğurla yaradıldı.</response>
        /// <response code="400">Göndərilən məlumatlar keçərsiz idi.</response>
        /// <response code="401">İstifadəçi autentifikasiya olunmayıb.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateBook(CreateBookDTO entity)
        {
            var result = await _bookService.AddAsync(entity);
            return StatusCode((int)result.StatusCode, result);
        }
        /// <summary>
        /// Mövcud kitabın məlumatlarını yeniləyir.
        /// </summary>
        /// <param name="id">Yenilənəcək kitabın identifikatoru.</param>
        /// <param name="entity">Yeni kitab məlumatları.</param>
        /// <response code="204">Kitab uğurla yeniləndi.</response>
        /// <response code="404">Bu ID ilə kitab tapılmadı.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBook(Guid id, UpdateBookDTO entity)
        {
            var result = await _bookService.UpdateAsync(id, entity);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Kitabı sistemdən silir. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <param name="id">Silinəcək kitabın identifikatoru.</param>
        /// <response code="204">Kitab uğurla silindi.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        /// <response code="404">Bu ID ilə kitab tapılmadı.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var result = await _bookService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
