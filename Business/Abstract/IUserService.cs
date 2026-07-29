using Core.Utilities.Results.Abstract;
using Entities.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IUserService
    {
        Task<IDataResult<List<GetUserDTO>>> GetAllAsync();

        Task<IDataResult<GetUserDTO>> GetByIdAsync(Guid id);

        Task<IResult> UpdateRoleAsync(Guid id, UpdateUserDTO entity);
    }
}
