using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.AuthorDTOS;
using Entities.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IAuthorService
    {
        Task<IDataResult<List<GetAuthorDTO>>> GetAllAsync(PaginationParameters pagination);
        /// <summary>
        /// Bax: IAuthorDAL.GetAllWithBooksAsync - N+1 sorğu problemi olmadan (tək sorğu,
        /// Include ilə) müəllifləri kitabları ilə birlikdə qaytarır.
        /// </summary>
        Task<IDataResult<List<GetAuthorWithBooksDTO>>> GetAllWithBooksAsync(PaginationParameters pagination);

        Task<IDataResult<GetAuthorDTO?>> GetByIdAsync(Guid id);

        Task<IResult> AddAsync(CreateAuthorDTO entity);

        Task<IResult> UpdateAsync(Guid id, UpdateAuthorDTO entity);

        Task<IResult> DeleteAsync(Guid id);
       
    }
}
