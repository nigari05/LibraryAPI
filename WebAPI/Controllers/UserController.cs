using Business.Abstract;
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

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllAsync();
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(Guid id, UpdateUserDTO entity)
        {
            var result = await _userService.UpdateRoleAsync(id, entity);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
