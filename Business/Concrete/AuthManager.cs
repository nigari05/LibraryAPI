using Business.Abstract;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Results.Concrete.SuccessResults;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserDAL _userDAL;
        private readonly IJWTService _jwtService;

        public AuthManager( IUserDAL userDAL, IJWTService jwtService)
        {
            _userDAL = userDAL;
            _jwtService = jwtService;
        }
        public async Task<IResult> RegisterAsync(RegisterDTO entity)
        {
            var existing = await _userDAL.GetByEmailAsync(entity.Email);
            if (existing != null)
                return new ErrorResult(HttpStatusCode.BadRequest, "This email is already registered.");

            AppUser user = new()
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                UserName = entity.UserName,
                Email = entity.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.Password),
                Role = "User" 
            };

            await _userDAL.AddAsync(user);

            return new SuccessResult(HttpStatusCode.Created);
        }

        public async Task<IDataResult<string>> LoginAsync(LoginDTO entity)
        {
            var user = await _userDAL.GetByEmailAsync(entity.Email);

            if (user == null)
                return new ErrorDataResult<string>(HttpStatusCode.NotFound, "User not found.");

            bool check = BCrypt.Net.BCrypt.Verify(entity.Password, user.PasswordHash);

            if (!check)
                return new ErrorDataResult<string>(HttpStatusCode.BadRequest, "Password is incorrect.");

            var token = _jwtService.GenerateToken(user);

            return new SuccessDataResult<string>(HttpStatusCode.OK, token);
        }

    }
}
