using Business.Abstract;
using Business.Concrete;
using Business.Validation.AuthorValidators;
using Business.Validation.BookValidators;
using Business.Validation.MemberValidators;
using DataAccess.Absract;
using DataAccess.Concrete.EntityFramework;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.DependencyResolver
{
    public static class ServiceRestration
    {
        public static void AddBusinessService(this IServiceCollection services)
        {

            services.AddScoped<AppDbContext>();
            services.AddScoped<IAuthorDAL, EfAuthorDAL>();
            services.AddScoped<IAuthorService, AuthorManager>();
            services.AddScoped<IMemberDAL, EfMemberDAL>();
            services.AddScoped<IMemberService, MemberManager>();
            services.AddScoped<IBookDAL, EfBookDAL>();
            services.AddScoped<IBookService, BookManager>();
            services.AddValidatorsFromAssemblyContaining<CreateBookValidator>();

        }
    }
}
