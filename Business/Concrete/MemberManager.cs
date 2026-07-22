using Business.Abstract;
using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Results.Concrete.SuccessResults;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.MemberDTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Business.Concrete
{
    public class MemberManager : IMemberService
    {
        private readonly IMemberDAL _memberDAL;

        public MemberManager(IMemberDAL memberDAL)
        {
            _memberDAL = memberDAL;
        }

        public async Task<IResult> AddAsync(CreateMemberDTO entity)
        {
            var member = new Member
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email
            };

            await _memberDAL.AddAsync(member);
            return new SuccessResult(HttpStatusCode.Created, "Member added successfully.");
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var member = await _memberDAL.GetByIdAsync(id);

            if (member == null)
                throw new KeyNotFoundException("Book not found.");

            await _memberDAL.DeleteAsync(member);
            return new SuccessResult(HttpStatusCode.NoContent, "Member deleted successfully.");
        }

        public async Task<IDataResult<List<GetMemberDTO>>> GetAllAsync(PaginationParameters pagination)
        {
            var members = await _memberDAL.GetAllAsync(pagination);

            var model = members.Select(member => new GetMemberDTO
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email
            }).ToList();
            return new SuccessDataResult<List<GetMemberDTO>>(HttpStatusCode.OK, "Members retrieved successfully.", model);
        }

        public async Task<IDataResult<GetMemberDTO?>> GetByIdAsync(Guid id)
        {
            var member = await _memberDAL.GetByIdAsync(id);

            if (member == null)
                throw new KeyNotFoundException("Book not found.");

            GetMemberDTO model = new GetMemberDTO
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email
            };
            return new SuccessDataResult<GetMemberDTO?>(HttpStatusCode.OK, "Member retrieved successfully.", model);
        }

        public async Task<IResult> UpdateAsync(Guid id, UpdateMemberDTO entity)
        {
            var member = await _memberDAL.GetByIdAsync(id);

            if (member == null)
                throw new KeyNotFoundException("Book not found.");

            member.FirstName = entity.FirstName;
            member.LastName = entity.LastName;
            member.Email = entity.Email;

            await _memberDAL.UpdateAsync(member);
            return new SuccessResult(HttpStatusCode.NoContent, "Member updated successfully.");
        }

    
    }
}
