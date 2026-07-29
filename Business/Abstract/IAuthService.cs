using Core.Utilities.Results.Abstract;
using Entities.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IAuthService
    {
        Task<IResult> RegisterAsync(RegisterDTO entity);
        Task<IDataResult<string>> LoginAsync(LoginDTO entity);
    }
}
