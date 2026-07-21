using Business.Abstract;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.MemberDTOs;
using System;
using System.Collections.Generic;
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

        public async Task AddAsync(CreateMemberDTO entity)
        {
            var member = new Member
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email
            };

            await _memberDAL.AddAsync(member);
        }

        public async Task DeleteAsync(Guid id)
        {
            var member = await _memberDAL.GetByIdAsync(id);

            if (member == null)
                throw new Exception("Member not found.");

            await _memberDAL.DeleteAsync(member);
        }

        public async Task<List<GetMemberDTO>> GetAllAsync()
        {
            var members = await _memberDAL.GetAllAsync();

            return members.Select(member => new GetMemberDTO
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email
            }).ToList();
        }

        public async Task<GetMemberDTO?> GetByIdAsync(Guid id)
        {
            var member = await _memberDAL.GetByIdAsync(id);

            if (member == null)
                return null;

            return new GetMemberDTO
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email
            };
        }

        public async Task UpdateAsync(Guid id, UpdateMemberDTO entity)
        {
            var member = await _memberDAL.GetByIdAsync(id);

            if (member == null)
                throw new Exception("Member not found.");

            member.FirstName = entity.FirstName;
            member.LastName = entity.LastName;
            member.Email = entity.Email;

            await _memberDAL.UpdateAsync(member);
        }
        }
    }
}
