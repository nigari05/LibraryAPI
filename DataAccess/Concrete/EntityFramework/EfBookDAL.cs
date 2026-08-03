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
    public class EfBookDAL : EfRepositorybase<Book, AppDbContext>, IBookDAL
    {

        private readonly AppDbContext _context;

        public EfBookDAL(AppDbContext context)
        {
            _context = context;
        }
        public async  Task<List<Book>> GetAllAsync(PaginationParameters pagination)
        {
            IQueryable<Book> query = _context.Books.Include(x => x.Author);

            if (!string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                switch (pagination.SortBy.ToLower())
                {
                    case "title":
                        query = pagination.IsDescending
                            ? query.OrderByDescending(x => x.Title)
                            : query.OrderBy(x => x.Title);
                        break;

                    case "price":
                        query = pagination.IsDescending
                            ? query.OrderByDescending(x => x.Price)
                            : query.OrderBy(x => x.Price);
                        break;
                }

            }

            return await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
        }
    }
}
