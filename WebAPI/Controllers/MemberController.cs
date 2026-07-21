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
            var members = await _memberService.GetAllAsync();
            return Ok(members);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(Guid id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null)
                return NotFound();
            return Ok(member);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberDTO entity)
        {
            await _memberService.AddAsync(entity);
            return Ok("Member added successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(Guid id, UpdateMemberDTO entity)
        {
            await _memberService.UpdateAsync(id, entity);
            return Ok("Member updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(Guid id)
        {
            await _memberService.DeleteAsync(id);
            return Ok("Member deleted successfully.");
        }
    }
}
