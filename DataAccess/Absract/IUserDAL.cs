using Core.DataAccess;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Absract
{
    public interface IUserDAL : IRepositorybase<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
