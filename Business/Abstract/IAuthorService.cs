using Core.Utilities.Results.Abstract;
using Entities.DTOs.AuthorDTOS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IAuthorService
    {
        Task<IDataResult<List<GetAuthorDTO>>> GetAllAsync();

        Task<IDataResult<GetAuthorDTO?>> GetByIdAsync(Guid id);

        Task<IResult> AddAsync(CreateAuthorDTO entity);

        Task<IResult> UpdateAsync(Guid id, UpdateAuthorDTO entity);

        Task<IResult> DeleteAsync(Guid id);
    }
}
