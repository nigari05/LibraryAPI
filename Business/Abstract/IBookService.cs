using Entities.DTOs.BookDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IBookService
    {
        Task<List<GetBookDTO>> GetAllBooksAsync();
         Task<GetBookDTO?> GetByIdAsync(Guid id);

        Task AddAsync(CreateBookDTO entity);

        Task UpdateAsync(Guid id, UpdateBookDTO entity);

        Task DeleteAsync(Guid id);

    }
}
