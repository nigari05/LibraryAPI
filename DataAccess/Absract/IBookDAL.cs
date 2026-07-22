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
        Task<List<Book>> GetAllAsync(PaginationParameters pagination);

    }
}
