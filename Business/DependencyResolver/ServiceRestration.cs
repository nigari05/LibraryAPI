using Business.Abstract;
using Business.Concrete;
using Business.Mapping;
using Business.Validation.AuthorValidators;
using Business.Validation.BookValidators;
using Business.Validation.MemberValidators;
using Core.Utilities.Caching;
using DataAccess.Absract;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Migrations;
using Entities.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
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
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();


            services.AddScoped<AppDbContext>();
            services.AddScoped<IAuthorDAL, EfAuthorDAL>();
            services.AddScoped<IAuthorService, AuthorManager>();
            services.AddScoped<IMemberDAL, EfMemberDAL>();
            services.AddScoped<IMemberService, MemberManager>();
            services.AddScoped<IBookDAL, EfBookDAL>();
            services.AddScoped<IBookService, BookManager>();
            services.AddValidatorsFromAssemblyContaining<CreateBookValidator>();

            services.AddScoped<IUserDAL, EfUserDAL>();
            services.AddScoped<IJWTService, JWTManager>();
            services.AddScoped<IAuthService, AuthManager>();
            services.AddScoped<IUserService, UserManager>();
            services.AddAutoMapper(cfg => { }, typeof(BookProfile).Assembly);
            services.AddScoped<ICategoryDAL, EfCategoryDAL>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IBookLoanDAL, EfBookLoanDAL>();
            services.AddScoped<IBookLoanService, BookLoanManager>();
        }
    }
}
