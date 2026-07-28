using Core.Utilities.Results.Abstract;
using Entities.DTOs.RoleDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IRoleService
    {
        Task<IDataResult<List<GetRoleDTO>>> GetAllAsync();

        Task<IDataResult<GetRoleDTO>> GetByIdAsync(string roleId);

        Task<IResult> CreateAsync(CreateRoleDTO entity);

        Task<IResult> UpdateAsync(string roleId, UpdateRoleDTO entity);

        Task<IResult> DeleteAsync(string roleId);

        Task<IResult> AssignRoleAsync(AssignRoleDTO entity);

        Task<IResult> RemoveRoleAsync(RemoveRoleDTO entity);


    }
}
