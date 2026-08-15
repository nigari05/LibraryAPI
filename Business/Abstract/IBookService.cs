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
        /// <summary>
        /// Mürəkkəb filtrlərə (başlıq, müəllif, kateqoriya, qiymət aralığı, stok) əsasən
        /// dinamik LINQ sorğusu ilə səhifələnmiş kitab siyahısını qaytarır.
        /// </summary>
        Task<IDataResult<PagedResult<GetBookDTO>>> GetAllBooksAsync(BookFilterParameters filterParameters);
        /// <summary>
        /// Native (raw) SQL sorğusu ilə açar söz, qiymət aralığı və kateqoriyaya görə axtarış edir.
        /// </summary>
        Task<IDataResult<List<GetBookDTO>>> SearchBooksNativeAsync(string? keyword, decimal? minPrice, decimal? maxPrice, Guid? categoryId);
        /// <summary>
        /// Specification pattern (BookFilterSpecification) əsasında dinamik axtarış/filtrasiya
        /// edir və nəticəni səhifələnmiş formada qaytarır.
        /// </summary>
        Task<IDataResult<PagedResult<GetBookDTO>>> FilterBooksAsync(BookFilterParameters filterParameters);
       
        Task<IDataResult<GetBookDTO?>> GetByIdAsync(Guid id);

        Task<IResult> AddAsync(CreateBookDTO entity);

        Task<IResult> UpdateAsync(Guid id, UpdateBookDTO entity);

        Task<IResult> DeleteAsync(Guid id);

        /// <summary>
        /// Kitabın üz qabığı şəklini yükləyir (multipart/form-data). Fayl tipi
        /// (.jpg/.jpeg/.png/.webp) və ölçüsü (maks. 5 MB) validasiya edilir.
        /// </summary>
        Task<IResult> UploadCoverImageAsync(Guid id, Microsoft.AspNetCore.Http.IFormFile file);

        /// <summary>
        /// Kitabın əvvəllər yüklənmiş üz qabığı şəklini (bytes) qaytarır.
        /// </summary>
        Task<IDataResult<BookCoverDTO>> DownloadCoverImageAsync(Guid id);

    }
}
