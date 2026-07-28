using Business.Abstract;
using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Results.Concrete.SuccessResults;
using DataAccess.Absract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.Concrete.UserDTOs;
using Entities.DTOs.AuthorDTOS;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Business.Concrete
{
    public class AuthorManager : IAuthorService
    {
        private readonly IAuthorDAL _authorDAL;
        private readonly IUserDAL _userDAL;

        public AuthorManager(IAuthorDAL authorDAL, IUserDAL userDAL)
        {
            _authorDAL = authorDAL;
            _userDAL = userDAL;
        }

        public async Task<IResult> AddAsync(CreateAuthorDTO entity)
        {
            var author = new Author
            {
                FullName = entity.FullName,
                Biography = entity.Biography
            };

            await _authorDAL.AddAsync(author);
            return new SuccessResult(HttpStatusCode.Created, "Author created successfully.");

        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var author = await _authorDAL.GetByIdAsync(id);

            if (author == null)
                throw new KeyNotFoundException("Book not found.");

            await _authorDAL.DeleteAsync(author);
            return new SuccessResult(HttpStatusCode.NoContent, "Author deleted successfully. ");
        }

        public async Task<IDataResult<List<GetAuthorDTO>>> GetAllAsync(PaginationParameters pagination)
        {
            var authors = await _authorDAL.GetAllAsync(pagination);

            List<GetAuthorDTO> models = authors.Select(author => new GetAuthorDTO
            {
                Id = author.Id,
                FullName = author.FullName,
                Biography = author.Biography
            }).ToList();
            return new SuccessDataResult<List<GetAuthorDTO>>(HttpStatusCode.OK, models);

        }

        public async Task<IDataResult<GetAuthorDTO>?> GetByIdAsync(Guid id)
        { 
            var author = await _authorDAL.GetByIdAsync(id);

            if (author == null)
                throw new KeyNotFoundException("Book not found.");


            GetAuthorDTO model = new() 
            {
                Id = author.Id,
                FullName = author.FullName,
                Biography = author.Biography
            };
            return new SuccessDataResult<GetAuthorDTO>(HttpStatusCode.OK, model);


        }

        public async Task<IResult> LoginAsync(LoginDTO entity)
        {
            var user = await _userDAL.GetByEmailAsync(entity.Email);

            if (user == null)
                return new ErrorResult(HttpStatusCode.NotFound, "User not found.");

            bool check = BCrypt.Net.BCrypt.Verify(entity.Password, user.PasswordHash);

            if (!check)
                return new ErrorResult(HttpStatusCode.BadRequest, "Password is incorrect.");

            return new SuccessResult(HttpStatusCode.OK, "Login successful.");
        }

        public async Task<IResult> RegisterAsync(RegisterDTO entity)
        {
            var user = new User
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                UserName = entity.UserName,
                Email = entity.Email,

                PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.Password)
            };

            await _userDAL.AddAsync(user);

            return new SuccessResult(HttpStatusCode.Created, "User registered successfully.");
        }

        public async Task<IResult> UpdateAsync(Guid id, UpdateAuthorDTO entity)
        {
            var author = await _authorDAL.GetByIdAsync(id);

            if (author == null)
                throw new KeyNotFoundException("Book not found.");


            author.FullName = entity.FullName;
            author.Biography = entity.Biography;

            await _authorDAL.UpdateAsync(author);
            return new SuccessResult(HttpStatusCode.NoContent, "Author updated successfully.");

        }
    }
}
