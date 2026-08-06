using Core.DataAccess.EntityFramework;
using Core.Specification;
using Core.Utilities.Pagination;
using DataAccess.Absract;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfBookDAL : EfRepositorybase<Book, AppDbContext>, IBookDAL
    {

        private readonly AppDbContext _context;

        public EfBookDAL(AppDbContext context) : base(context) 
        {
            _context = context;
        }
        /// <summary>
        /// Derived query method məntiqi: hər bir filter sahəsi (Title, AuthorName,
        /// CategoryId, MinPrice, MaxPrice, InStockOnly) yalnız dolu olduqda sorğuya
        /// əlavə edilir - Spring Data-dakı findByTitleContainingAndPriceBetween(...)
        /// kimi adlandırılmış metodların LINQ üzərində dinamik ekvivalenti.
        /// </summary>
        public async Task<(List<Book> Books, int TotalCount)> GetAllAsync(BookFilterParameters filter)
        {
            IQueryable<Book> query = _context.Books
                .Include(x => x.Author)
                .Include(x => x.Categories)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Title))
                query = query.Where(b => b.Title.Contains(filter.Title));

            if (!string.IsNullOrWhiteSpace(filter.AuthorName))
                query = query.Where(b => b.Author != null && b.Author.FullName.Contains(filter.AuthorName));

            if (filter.CategoryId.HasValue)
                query = query.Where(b => b.Categories.Any(c => c.Id == filter.CategoryId.Value));

            if (filter.MinPrice.HasValue)
                query = query.Where(b => b.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(b => b.Price <= filter.MaxPrice.Value);

            if (filter.InStockOnly == true)
                query = query.Where(b => b.Stock > 0);

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "title":
                        query = filter.IsDescending
                            ? query.OrderByDescending(x => x.Title)
                            : query.OrderBy(x => x.Title);
                        break;
                    case "price":
                        query = filter.IsDescending
                            ? query.OrderByDescending(x => x.Price)
                            : query.OrderBy(x => x.Price);
                        break;
                    case "stock":
                        query = filter.IsDescending
                            ? query.OrderByDescending(x => x.Stock)
                            : query.OrderBy(x => x.Stock);
                        break;
                    default:
                        query = query.OrderBy(x => x.Title);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Title);
            }

            var totalCount = await query.CountAsync();

            var books = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return (books, totalCount);
        }

        /// <summary>
        /// Specification-u (Criteria + Includes + sıralama + səhifələnmə) IQueryable üzərinə
        /// tətbiq edir. Ümumi say (TotalCount) səhifələnmədən əvvəlki sorğu üzərindən,
        /// nəticə isə səhifələnmiş sorğu üzərindən alınır.
        /// </summary>
        public async Task<(List<Book> Books, int TotalCount)> GetBySpecificationAsync(ISpecification<Book> specification)
        {
            var totalCount = await SpecificationEvaluator<Book>
               .GetQuery(_context.Books.AsQueryable(), specification, applyPaging: false)
               .CountAsync();

            var books = await SpecificationEvaluator<Book>
                .GetQuery(_context.Books.AsQueryable(), specification, applyPaging: true)
                .AsNoTracking()
                .ToListAsync();

            return (books, totalCount);
        }

        /// <summary>
        /// Native SQL sorğusu: Books, Authors və BookCategories cədvəllərini birbaşa
        /// JOIN edərək açar söz (title/author üzrə), qiymət aralığı və kateqoriyaya
        /// görə axtarış aparır. Parametrlər FromSqlInterpolated vasitəsilə
        /// parametrləşdirilir (SQL injection-a qarşı təhlükəsizdir).
        /// </summary>
        public async Task<List<Book>> SearchBooksNativeAsync(string? keyword, decimal? minPrice, decimal? maxPrice, Guid? categoryId)
        {
            var min = minPrice ?? 0m;
            var max = maxPrice ?? 999999999m;
            var likePattern = string.IsNullOrWhiteSpace(keyword) ? null : $"%{keyword.Trim()}%";

            FormattableString sql = $@"
                SELECT DISTINCT b.*
                FROM Books b
                INNER JOIN Authors a ON a.Id = b.AuthorId
                LEFT JOIN BookCategories bc ON bc.BookId = b.Id
                WHERE b.Price BETWEEN {min} AND {max}
                  AND ({likePattern} IS NULL OR b.Title LIKE {likePattern} OR a.FullName LIKE {likePattern})
                  AND ({categoryId} IS NULL OR bc.CategoryId = {categoryId})";

            return await _context.Books
                .FromSqlInterpolated(sql)
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
