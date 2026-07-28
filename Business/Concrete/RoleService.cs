using Business.Abstract;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Results.Concrete.SuccessResults;
using Entities.Concrete;
using Entities.DTOs.RoleDTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Business.Concrete
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleService(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IResult> AssignRoleAsync(AssignRoleDTO entity)
        {
            var user = await _userManager.FindByIdAsync(entity.UserId.ToString());

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (!await _roleManager.RoleExistsAsync(entity.RoleName))
                throw new KeyNotFoundException("Role not found.");

            var result = await _userManager.AddToRoleAsync(user, entity.RoleName);

            if (!result.Succeeded)
                return new ErrorResult(HttpStatusCode.BadRequest, "Role could not be assigned.");

            return new SuccessResult(HttpStatusCode.OK, "Role assigned successfully.");
        }

        public async Task<IResult> CreateAsync(CreateRoleDTO entity)
        {
            var role = new IdentityRole
            {
                Name = entity.Name
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                return new ErrorResult(HttpStatusCode.BadRequest, "Role could not be created.");

            return new SuccessResult(HttpStatusCode.Created, "Role created successfully.");
        }

        public async Task<IResult> DeleteAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
                throw new KeyNotFoundException("Role not found.");

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                return new ErrorResult(HttpStatusCode.BadRequest, "Role could not be deleted.");

            return new SuccessResult(HttpStatusCode.NoContent, "Role deleted successfully.");
        }

        public async  Task<IDataResult<List<GetRoleDTO>>> GetAllAsync()
        {
            var roles =  _roleManager.Roles.ToList();

            var models = roles.Select(x => new GetRoleDTO
            {
                Id = x.Id,
                Name = x.Name!
            }).ToList();

            return new SuccessDataResult<List<GetRoleDTO>>(HttpStatusCode.OK, models);
        }

        public async Task<IDataResult<GetRoleDTO>> GetByIdAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
                throw new KeyNotFoundException("Role not found.");

            var models = new GetRoleDTO
            {
                Id = role.Id,
                Name = role.Name!
            };

            return new SuccessDataResult<GetRoleDTO>(HttpStatusCode.OK, models);
        }

        public async Task<IResult> RemoveRoleAsync(RemoveRoleDTO entity)
        {
            var user = await _userManager.FindByIdAsync(entity.Id.ToString());

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var result = await _userManager.RemoveFromRoleAsync(user, entity.RoleName);

            if (!result.Succeeded)
                return new ErrorResult(HttpStatusCode.BadRequest, "Role could not be removed.");

            return new SuccessResult(HttpStatusCode.OK, "Role removed successfully.");
        }

        public async Task<IResult> UpdateAsync(string roleId, UpdateRoleDTO entity)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
                throw new KeyNotFoundException("Role not found.");

            role.Name = entity.Name;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                return new ErrorResult(HttpStatusCode.BadRequest, "Role could not be updated.");

            return new SuccessResult(HttpStatusCode.NoContent, "Role updated successfully.");
        }
    }
}
