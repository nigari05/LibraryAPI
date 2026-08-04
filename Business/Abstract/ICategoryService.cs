using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface ICategoryService
    {
        Task<IDataResult<List<GetCategoryDTO>>> GetAllCategoriesAsync(PaginationParameters pagination);
        Task<IDataResult<GetCategoryDTO?>> GetByIdAsync(Guid id);
        Task<IResult> AddAsync(CreateCategoryDTO entity);
        Task<IResult> UpdateAsync(Guid id, UpdateCategoryDTO entity);
        Task<IResult> DeleteAsync(Guid id);
    }
}
