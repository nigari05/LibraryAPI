using Core.DataAccess.EntityFramework;
using DataAccess.Absract;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDAL : EfRepositorybase<User, AppDbContext>, IUserDAL
    {
        public  async Task<User?> GetByEmailAsync(string email)
        {
             using AppDbContext context = new();

           return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
