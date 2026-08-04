using Business.Abstract;
using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.CategoryDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Bütün kateqoriyaları səhifələnmiş şəkildə qaytarır.
        /// </summary>
        /// <param name="pagination">Səhifə nömrəsi, ölçüsü və sıralama parametrləri.</param>
        /// <response code="200">Kateqoriya siyahısı uğurla qaytarıldı.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IDataResult<List<GetCategoryDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories([FromQuery] PaginationParameters pagination)
        {
            var result = await _categoryService.GetAllCategoriesAsync(pagination);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// ID-yə görə tək bir kateqoriyanı qaytarır.
        /// </summary>
        /// <param name="id">Kateqoriyanın unikal identifikatoru.</param>
        /// <response code="200">Kateqoriya tapıldı.</response>
        /// <response code="404">Bu ID ilə kateqoriya tapılmadı.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IDataResult<GetCategoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Yeni kateqoriya əlavə edir. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <param name="entity">Yaradılacaq kateqoriyanın məlumatları.</param>
        /// <response code="201">Kateqoriya uğurla yaradıldı.</response>
        /// <response code="400">Göndərilən məlumatlar keçərsiz idi.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateCategory(CreateCategoryDTO entity)
        {
            var result = await _categoryService.AddAsync(entity);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Mövcud kateqoriyanın adını yeniləyir. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <param name="id">Yenilənəcək kateqoriyanın identifikatoru.</param>
        /// <param name="entity">Yeni kateqoriya məlumatları.</param>
        /// <response code="204">Kateqoriya uğurla yeniləndi.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        /// <response code="404">Bu ID ilə kateqoriya tapılmadı.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryDTO entity)
        {
            var result = await _categoryService.UpdateAsync(id, entity);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Kateqoriyanı sistemdən silir. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <param name="id">Silinəcək kateqoriyanın identifikatoru.</param>
        /// <response code="204">Kateqoriya uğurla silindi.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        /// <response code="404">Bu ID ilə kateqoriya tapılmadı.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}

