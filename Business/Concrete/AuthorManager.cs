using Business.Abstract;
using Core.Utilities.Results.Abstract;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.AuthorDTOS;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Core.Utilities.Results.Concrete.SuccessResults;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Pagination;

namespace Business.Concrete
{
    public class AuthorManager : IAuthorService
    {
        private readonly IAuthorDAL _authorDAL;

        public AuthorManager(IAuthorDAL authorDAL)
        {
            _authorDAL = authorDAL;
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
