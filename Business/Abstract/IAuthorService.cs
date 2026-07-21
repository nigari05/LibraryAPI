using Entities.DTOs.AuthorDTOS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IAuthorService
    {
        Task<List<GetAuthorDTO>> GetAllAsync();

        Task<GetAuthorDTO?> GetByIdAsync(Guid id);

        Task AddAsync(CreateAuthorDTO entity);

        Task UpdateAsync(Guid id, UpdateAuthorDTO entity);

        Task DeleteAsync(Guid id);
    }
}
