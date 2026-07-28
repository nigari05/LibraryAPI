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
    internal class AuthManager : IAuthService
    {
        private readonly IAuthService _authService;
        private readonly IUserDAL _userDAL;
        private readonly IJWTService _jwtService;

        public AuthManager(IAuthService authService, IUserDAL userDAL, IJWTService jwtService)
        {
            _authService = authService;
            _userDAL = userDAL;
            _jwtService = jwtService;
        }
        public async Task<IResult> LoginAsync(LoginDTO entity)

        {
            var user = await _userDAL.GetByEmailAsync(entity.Email);

            if (user == null)
                return new ErrorResult(HttpStatusCode.NotFound, "User not found.");

            bool check = BCrypt.Net.BCrypt.Verify(entity.Password, user.PasswordHash);

            if (!check)
                return new ErrorResult(HttpStatusCode.BadRequest, "Password is incorrect.");

            var token = _jwtService.GenerateToken(user);

            return new SuccessDataResult<string>(HttpStatusCode.OK, token);
        }

        public async Task<IResult> RegisterAsync(RegisterDTO entity)
        {
            var user = new User
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                UserName = entity.UserName,
                Email = entity.Email,

                PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.Password),
                Role = "User"
            };

            await _userDAL.AddAsync(user);

            return new SuccessResult(HttpStatusCode.Created, "User registered successfully.");
        }

       
    }
}
