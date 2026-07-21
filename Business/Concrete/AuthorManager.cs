using Business.Abstract;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.AuthorDTOS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class AuthorManager : IAuthorService
    {
        private readonly IAuthorDAL _authorDAL;

        public AuthorManager(IAuthorDAL authorDAL)
        {
            _authorDAL = authorDAL;
        }

        public async Task AddAsync(CreateAuthorDTO entity)
        {
            var author = new Author
            {
                FullName = entity.FullName,
                Biography = entity.Biography
            };

            await _authorDAL.AddAsync(author);
        }

        public async Task DeleteAsync(Guid id)
        {
            var author = await _authorDAL.GetByIdAsync(id);

            if (author == null)
                throw new Exception("Author not found.");

            await _authorDAL.DeleteAsync(author);
        }

        public async Task<List<GetAuthorDTO>> GetAllAsync()
        {
            var authors = await _authorDAL.GetAllAsync();

            return authors.Select(author => new GetAuthorDTO
            {
                Id = author.Id,
                FullName = author.FullName,
                Biography = author.Biography
            }).ToList();
        }

        public async Task<GetAuthorDTO?> GetByIdAsync(Guid id)
        { 
            var author = await _authorDAL.GetByIdAsync(id);

            if (author == null)
                return null;

            return new GetAuthorDTO
            {
                Id = author.Id,
                FullName = author.FullName,
                Biography = author.Biography
            };
        }

        public async Task UpdateAsync(Guid id, UpdateAuthorDTO entity)
        {
            var author = await _authorDAL.GetByIdAsync(id);

            if (author == null)
                throw new Exception("Author not found.");

            author.FullName = entity.FullName;
            author.Biography = entity.Biography;

            await _authorDAL.UpdateAsync(author);
        }
    }
}
