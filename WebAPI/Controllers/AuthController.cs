using Business.Abstract;
using Entities.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Yeni istifadəçi qeydiyyatı yaradır.
        /// </summary>
        /// <param name="entity">Qeydiyyat üçün istifadəçi məlumatları.</param>
        /// <response code="201">İstifadəçi uğurla qeydiyyatdan keçdi.</response>
        /// <response code="400">Göndərilən məlumatlar keçərsiz idi (məs. email artıq mövcuddur).</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterDTO entity)
        {
            var result = await _authService.RegisterAsync(entity);

            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// İstifadəçi girişini həyata keçirir və JWT token qaytarır.
        /// </summary>
        /// <param name="entity">Giriş üçün email və şifrə.</param>
        /// <response code="200">Giriş uğurludur, JWT token qaytarıldı.</response>
        /// <response code="401">Email və ya şifrə yanlışdır.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginDTO entity)
        {
            var result = await _authService.LoginAsync(entity);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
