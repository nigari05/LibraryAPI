using AutoMapper;
using Business.Abstract;
using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Results.Concrete.SuccessResults;
using DataAccess.Absract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.DTOs.AuthorDTOS;
using Entities.DTOs.BookDTOs;
using Entities.DTOs.UserDTOs;
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
        private readonly IMapper _mapper;

        public AuthorManager(IAuthorDAL authorDAL)
        {
            _authorDAL = authorDAL;
        }

        public async Task<IResult> AddAsync(CreateAuthorDTO entity)
        {
            var author = _mapper.Map<Author>(entity);


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
            var authorDTOs = _mapper.Map<List<GetAuthorDTO>>(authors);


           
            return new SuccessDataResult<List<GetAuthorDTO>>(HttpStatusCode.OK, authorDTOs);

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

     

        public async Task<IResult> UpdateAsync(Guid id, UpdateAuthorDTO entity)
        {
            var author = await _authorDAL.GetByIdAsync(id);

            if (author == null)
                throw new KeyNotFoundException("Book not found.");


            _mapper.Map(entity, author);


            await _authorDAL.UpdateAsync(author);
            return new SuccessResult(HttpStatusCode.NoContent, "Author updated successfully.");

        }
    }
}
