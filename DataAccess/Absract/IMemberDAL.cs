using Core.DataAccess;
using Core.Utilities.Pagination;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Absract
{
    public interface IMemberDAL : IRepositorybase<Member>
    {
        Task<List<Member>> GetAllAsync(PaginationParameters pagination);

    }
}
