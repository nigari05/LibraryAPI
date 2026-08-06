using Core.DataAccess;
using Core.Utilities.Pagination;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Absract
{
    public interface IAuthorDAL : IRepositorybase<Author>
    {
        Task<List<Author>> GetAllAsync(PaginationParameters pagination);
        /// <summary>
        /// N+1 sorğu probleminin qarşısını almaq üçün müəllifləri Books əlaqəsi ilə
        /// BİRLİKDƏ (Include - EF Core-un JOIN FETCH/@EntityGraph ekvivalenti), TƏK bir
        /// SQL sorğusu ilə gətirir. Əks halda hər müəllif üçün ayrıca "SELECT * FROM Books
        /// WHERE AuthorId = ..." sorğusu lazım olardı (1 + N sorğu).
        /// </summary>
        Task<List<Author>> GetAllWithBooksAsync(PaginationParameters pagination);

    }
}
