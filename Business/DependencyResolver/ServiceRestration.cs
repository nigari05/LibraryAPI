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
    public class ServiceRestration
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IAuthorDAL, EfAuthorDAL>();
            services.AddScoped<IMemberDAL, EfMemberDAL>();
            services.AddScoped<IBookDAL, EfBookDAL>();
            services.AddValidatorsFromAssemblyContaining<CreateBookValidator>();
            
        }
    }
