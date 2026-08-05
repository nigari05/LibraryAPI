using Core.DataAccess;
using Core.DataAccess.Entities;
using Core.Utilities.Pagination;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Absract
{
    public interface IBookDAL : IRepositorybase<Book>
    {
        /// <summary>
        /// Dinamik LINQ filtrasiyası (derived query method məntiqi) ilə səhifələnmiş
        /// kitab siyahısını qaytarır.
        /// </summary>
        Task<(List<Book> Books, int TotalCount)> GetAllAsync(BookFilterParameters filter);

        /// <summary>
        /// Native (raw) SQL sorğusu ilə açar söz, qiymət aralığı və kateqoriyaya görə
        /// kitabları axtarır. Author və BookCategories cədvəlləri ilə JOIN edilir.
        /// </summary>
        Task<List<Book>> SearchBooksNativeAsync(string? keyword, decimal? minPrice, decimal? maxPrice, Guid? categoryId);


    }
}
