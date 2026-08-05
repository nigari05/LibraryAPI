using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.BookDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IBookService
    {
        Task<IDataResult<PagedResult<GetBookDTO>>> GetAllBooksAsync(BookFilterParameters filterParameters);
        Task<IDataResult<List<GetBookDTO>>> SearchBooksNativeAsync(string? keyword, decimal? minPrice, decimal? maxPrice, Guid? categoryId);
         Task<IDataResult<GetBookDTO?>> GetByIdAsync(Guid id);

        Task<IResult> AddAsync(CreateBookDTO entity);

        Task<IResult> UpdateAsync(Guid id, UpdateBookDTO entity);

        Task<IResult> DeleteAsync(Guid id);

    }
}
