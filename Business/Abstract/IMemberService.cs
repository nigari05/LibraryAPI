using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.MemberDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IMemberService
    {
        Task<IDataResult<List<GetMemberDTO>>> GetAllAsync(PaginationParameters pagination);


        Task<IDataResult<GetMemberDTO?>> GetByIdAsync(Guid id);

        Task<IResult> AddAsync(CreateMemberDTO entity);

        Task<IResult> UpdateAsync(Guid id, UpdateMemberDTO entity);

        Task<IResult> DeleteAsync(Guid id);
    }
}
