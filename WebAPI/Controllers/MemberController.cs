using Business.Abstract;
using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.MemberDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }


        /// <summary>
        /// Bütün üzvləri səhifələnmiş şəkildə qaytarır.
        /// </summary>
        /// <param name="pagination">Səhifə nömrəsi, ölçüsü və sıralama parametrləri.</param>
        /// <response code="200">Üzv siyahısı uğurla qaytarıldı.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IDataResult<List<GetMemberDTO>>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetAllMembers([FromQuery] PaginationParameters pagination)
        {
            var result = await _memberService.GetAllAsync(pagination);
            return StatusCode((int)result.StatusCode, result);
        }


        /// <summary>
        /// ID-yə görə tək bir üzvü qaytarır.
        /// </summary>
        /// <param name="id">Üzvün unikal identifikatoru.</param>
        /// <response code="200">Üzv tapıldı.</response>
        /// <response code="404">Bu ID ilə üzv tapılmadı.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IDataResult<GetMemberDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMemberById(Guid id)
        {
            var result = await _memberService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Yeni üzv qeydiyyata alır.
        /// </summary>
        /// <param name="entity">Yaradılacaq üzvün məlumatları.</param>
        /// <response code="201">Üzv uğurla yaradıldı.</response>
        /// <response code="400">Göndərilən məlumatlar keçərsiz idi.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMember(CreateMemberDTO entity)
        {
            var result = await _memberService.AddAsync(entity);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Mövcud üzvün məlumatlarını yeniləyir.
        /// </summary>
        /// <param name="id">Yenilənəcək üzvün identifikatoru.</param>
        /// <param name="entity">Yeni üzv məlumatları.</param>
        /// <response code="204">Üzv uğurla yeniləndi.</response>
        /// <response code="404">Bu ID ilə üzv tapılmadı.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMember(Guid id, UpdateMemberDTO entity)
        {
            var result = await _memberService.UpdateAsync(id, entity);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Üzvü sistemdən silir.
        /// </summary>
        /// <param name="id">Silinəcək üzvün identifikatoru.</param>
        /// <response code="204">Üzv uğurla silindi.</response>
        /// <response code="404">Bu ID ilə üzv tapılmadı.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMember(Guid id)
        {
            var result = await _memberService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
