using Business.Abstract;
using Business.Concrete;
using Business.Mapping;
using Business.Validation.AuthorValidators;
using Business.Validation.BookValidators;
using Business.Validation.MemberValidators;
using Core.Utilities.BackgroundTasks;
using Core.Utilities.Caching;
using Core.Utilities.FileStorage;
using DataAccess.Absract;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Migrations;
using Entities.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;


namespace Business.DependencyResolver
{
    public static class ServiceRestration
    {
        public static void AddBusinessService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();

            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
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
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<ICleanupService, CleanupManager>();

            // Checkpoint 4 - Asinxron emal (@Async). Növbə Singleton olmalıdır ki, bütün
            // scoped request-lər (BookLoanManager və s.) eyni növbəni paylaşsın.
            services.AddSingleton<IBackgroundTaskQueue>(_ => new BackgroundTaskQueue(capacity: 100));
            services.AddScoped<IEmailNotificationService, EmailNotificationManager>();
        }
    }
}
