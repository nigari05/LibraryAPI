using Business.Abstract;
using Entities.DTOs.RoleDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleService.GetAllAsync();
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetById(string roleId)
        {
            var result = await _roleService.GetByIdAsync(roleId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleDTO dto)
        {
            var result = await _roleService.CreateAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> Update(string roleId, UpdateRoleDTO dto)
        {
            var result = await _roleService.UpdateAsync(roleId, dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> Delete(string roleId)
        {
            var result = await _roleService.DeleteAsync(roleId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(AssignRoleDTO dto)
        {
            var result = await _roleService.AssignRoleAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole(RemoveRoleDTO dto)
        {
            var result = await _roleService.RemoveRoleAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
