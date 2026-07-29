using Business.Abstract;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Results.Concrete.SuccessResults;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.AuthorDTOS;
using Entities.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Business.Concrete
{
    public   class UserManager : IUserService
    {
        private static readonly string[] AllowedRoles = { "Admin", "User" };

        private readonly IUserDAL _userDAL;

        public UserManager(IUserDAL userDAL)
        {
            _userDAL = userDAL;
        }

        public async Task<IDataResult<List<GetUserDTO>>> GetAllAsync()
        {
            var users = await _userDAL.GetAllAsync();

            List<GetUserDTO> result = users.Select(user => new GetUserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Role = user.Role

            }).ToList();

            return new SuccessDataResult<List<GetUserDTO>>(HttpStatusCode.OK, result);
        }

        public async Task<IDataResult<GetUserDTO>> GetByIdAsync(Guid id)
        {
            var user = await _userDAL.GetByIdAsync(id);

            if (user == null)
                return new ErrorDataResult<GetUserDTO>(HttpStatusCode.NotFound, "User not found.");

           var result = new GetUserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
            return new SuccessDataResult<GetUserDTO>(HttpStatusCode.OK, result);
        }

        public async Task<IResult> UpdateRoleAsync(Guid id, UpdateUserDTO entity)
        {
            if (!AllowedRoles.Contains(entity.Role))
                return new ErrorResult(HttpStatusCode.BadRequest, "Role must be either 'Admin' or 'User'.");

            var user = await _userDAL.GetByIdAsync(id);

            if (user == null)
                return new ErrorResult(HttpStatusCode.NotFound, "User not found.");

            user.Role = entity.Role;

            await _userDAL.UpdateAsync(user);

            return new SuccessResult(HttpStatusCode.OK, "User role updated successfully.");
        }
    }
}
