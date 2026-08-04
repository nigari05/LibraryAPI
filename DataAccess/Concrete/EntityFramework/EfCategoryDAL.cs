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
    public class EfCategoryDAL : EfRepositorybase<Category, AppDbContext>, ICategoryDAL
    {
        private readonly AppDbContext _context;

        public EfCategoryDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync(PaginationParameters pagination)
        {
            IQueryable<Category> query = _context.Categories.Include(x => x.Books);

            if (!string.IsNullOrWhiteSpace(pagination.SortBy) &&
                pagination.SortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                query = pagination.IsDescending
                    ? query.OrderByDescending(x => x.Name)
                    : query.OrderBy(x => x.Name);
            }

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
    }
}
