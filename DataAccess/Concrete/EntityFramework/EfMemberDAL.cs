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
    public class EfMemberDAL : EfRepositorybase<Member, AppDbContext>, IMemberDAL
    {
        public async Task<List<Member>> GetAllAsync(PaginationParameters pagination)
        {
            using AppDbContext context = new();

            IQueryable<Member> query = context.Members;

            if (!string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                switch (pagination.SortBy.ToLower())
                {
                    case "firstname":
                        query = pagination.IsDescending
                            ? query.OrderByDescending(x => x.FirstName)
                            : query.OrderBy(x => x.FirstName);
                        break;

                    case "lastname":
                        query = pagination.IsDescending
                            ? query.OrderByDescending(x => x.LastName)
                            : query.OrderBy(x => x.LastName);
                        break;

                    case "email":
                        query = pagination.IsDescending
                            ? query.OrderByDescending(x => x.Email)
                            : query.OrderBy(x => x.Email);
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
