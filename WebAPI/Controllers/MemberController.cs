using Business.Abstract;
using Entities.DTOs.MemberDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            var result = await _memberService.GetAllAsync();
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(Guid id)
        {
            var result = await _memberService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberDTO entity)
        {
            var result = await _memberService.AddAsync(entity);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(Guid id, UpdateMemberDTO entity)
        {
            var result = await _memberService.UpdateAsync(id, entity);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(Guid id)
        {
            var result = await _memberService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
