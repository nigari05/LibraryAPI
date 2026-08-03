using Business.Abstract;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Sistemdəki bütün istifadəçiləri qaytarır. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <response code="200">İstifadəçi siyahısı uğurla qaytarıldı.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IDataResult<List<GetUserDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllAsync();
            return StatusCode((int)result.StatusCode, result);
        }
        /// <summary>
        /// ID-yə görə tək bir istifadəçini qaytarır. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <param name="id">İstifadəçinin unikal identifikatoru.</param>
        /// <response code="200">İstifadəçi tapıldı.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        /// <response code="404">Bu ID ilə istifadəçi tapılmadı.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IDataResult<GetUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// İstifadəçinin rolunu yeniləyir. Yalnız Admin rolu icazəlidir.
        /// </summary>
        /// <param name="id">Rolu yenilənəcək istifadəçinin identifikatoru.</param>
        /// <param name="entity">Yeni rol məlumatı.</param>
        /// <response code="204">Rol uğurla yeniləndi.</response>
        /// <response code="403">İstifadəçinin Admin rolu yoxdur.</response>
        /// <response code="404">Bu ID ilə istifadəçi tapılmadı.</response>
        [HttpPut("{id}/role")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserRole(Guid id, UpdateUserDTO entity)
        {
            var result = await _userService.UpdateRoleAsync(id, entity);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
