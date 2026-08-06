using Core.DataAccess.EntityFramework;
using Core.Utilities.Pagination;
using DataAccess.Absract;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfAuthorDAL : EfRepositorybase<Author, AppDbContext>, IAuthorDAL
    {
        public async Task<List<Author>> GetAllAsync(PaginationParameters pagination)
        {
            using AppDbContext context = new();

            IQueryable<Author> query = context.Authors;

            if (!string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                switch (pagination.SortBy.ToLower())
                {
                    case "fullname":
                        query = pagination.IsDescending
                            ? query.OrderByDescending(x => x.FullName)
                            : query.OrderBy(x => x.FullName);
                        break;
                }
            }

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Bax: IAuthorDAL.GetAllWithBooksAsync. .Include(a =&gt; a.Books) sayəsində EF Core
        /// bunu TƏK bir SQL sorğusu (JOIN) kimi generasiya edir - N+1 problemi olmadan.
        /// </summary>
        public async Task<List<Author>> GetAllWithBooksAsync(PaginationParameters pagination)
        {
            using AppDbContext context = new();

            IQueryable<Author> query = context.Authors.Include(a => a.Books);

            if (!string.IsNullOrWhiteSpace(pagination.SortBy) && pagination.SortBy.ToLower() == "fullname")
            {
                query = pagination.IsDescending
                    ? query.OrderByDescending(x => x.FullName)
                    : query.OrderBy(x => x.FullName);
            }
            else
            {
                query = query.OrderBy(x => x.FullName);
            }

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
