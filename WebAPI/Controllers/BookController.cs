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

        public async Task<IActionResult> GetAllBooks([FromQuery] BookFilterParameters filterParameters)
        {
            var result = await _bookService.GetAllBooksAsync(filterParameters);
            return StatusCode((int)result.StatusCode, result);
        }
        /// <summary>
        /// Açar söz (başlıq/müəllif üzrə), qiymət aralığı və kateqoriyaya görə native
        /// (raw) SQL sorğusu ilə axtarış aparır. Books, Authors və BookCategories
        /// cədvəlləri birbaşa JOIN edilir.
        /// </summary>
        /// <param name="keyword">Başlıq və ya müəllif adında axtarılacaq açar söz (opsional).</param>
        /// <param name="minPrice">Minimum qiymət (opsional).</param>
        /// <param name="maxPrice">Maksimum qiymət (opsional).</param>
        /// <param name="categoryId">Kateqoriya ID-si (opsional).</param>
        /// <response code="200">Axtarış nəticələri uğurla qaytarıldı.</response>
        [HttpGet("search")]
        [Authorize]
        [ProducesResponseType(typeof(IDataResult<List<GetBookDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchBooksNative(
            [FromQuery] string? keyword,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] Guid? categoryId)
        {
            var result = await _bookService.SearchBooksNativeAsync(keyword, minPrice, maxPrice, categoryId);
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
        [Authorize(Roles = "Admin")]
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


        /// <summary>
        /// Specification pattern (BookFilterSpecification) əsasında dinamik axtarış/filtrasiya
        /// aparır. Checkpoint 2-dəki endpoint-dən fərqli olaraq, filtr məntiqi birbaşa DAL-da
        /// deyil, ayrıca, yenidən istifadə oluna bilən Specification obyektində təsvir olunur.
        /// </summary>
        /// <param name="filter">Səhifələnmə, sıralama və filtr parametrləri (Title, AuthorName, CategoryId, MinPrice, MaxPrice, InStockOnly).</param>
        /// <response code="200">Filtrlənmiş kitab siyahısı uğurla qaytarıldı.</response>
        [HttpGet("filter")]
        [Authorize]
        [ProducesResponseType(typeof(IDataResult<PagedResult<GetBookDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterBooks([FromQuery] BookFilterParameters filter)
        {
            var result = await _bookService.FilterBooksAsync(filter);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Kitabın üz qabığı şəklini yükləyir (multipart/form-data). Yalnız Admin rolu
        /// icazəlidir. Fayl tipi (.jpg/.jpeg/.png/.webp) və ölçüsü (maks. 5 MB) validasiya edilir.
        /// </summary>
        /// <param name="id">Kitabın identifikatoru.</param>
        /// <param name="file">Yüklənəcək şəkil faylı.</param>
        /// <response code="200">Şəkil uğurla yükləndi.</response>
        /// <response code="400">Fayl seçilməyib, ölçüsü limitdən böyükdür və ya formatı dəstəklənmir.</response>
        /// <response code="401">İstifadəçi autentifikasiya olunmayıb.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        /// <response code="404">Bu ID ilə kitab tapılmadı.</response>
        [HttpPost("{id}/cover")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Core.Utilities.Results.Abstract.IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadCoverImage(Guid id, IFormFile file)
        {
            var result = await _bookService.UploadCoverImageAsync(id, file);
            return StatusCode((int)result.StatusCode, result);
        }


        /// <summary>
        /// Kitabın əvvəllər yüklənmiş üz qabığı şəklini fayl olaraq endirir.
        /// </summary>
        /// <param name="id">Kitabın identifikatoru.</param>
        /// <response code="200">Şəkil faylı qaytarıldı.</response>
        /// <response code="404">Bu ID ilə kitab, ya da ona aid şəkil tapılmadı.</response>
        [HttpGet("{id}/cover")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadCoverImage(Guid id)
        {
            var result = await _bookService.DownloadCoverImageAsync(id);

            if (!result.Success || result.Data == null)
                return StatusCode((int)result.StatusCode, result);

            return File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
        }
    }
}
