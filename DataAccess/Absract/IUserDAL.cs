using Core.DataAccess;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Absract
{
    public interface IUserDAL : IRepositorybase<AppUser>
    {
        Task<AppUser?> GetByEmailAsync(string email);
    }
}
