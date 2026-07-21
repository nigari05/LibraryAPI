using Entities.DTOs.MemberDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IMemberService
    {
        Task<List<GetMemberDTO>> GetAllAsync();

        Task<GetMemberDTO?> GetByIdAsync(Guid id);

        Task AddAsync(CreateMemberDTO entity);

        Task UpdateAsync(Guid id, UpdateMemberDTO entity);

        Task DeleteAsync(Guid id);
    }
}
