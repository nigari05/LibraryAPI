using Entities.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class AppDbContext : DbContext
    {

        // Parametrsiz constructor - mövcud "new TContext()" əsaslı EfRepositorybase
        // (bax: Core/DataAccess/EntityFramework/EfRepositorybase.cs) üçün saxlanılır.
     
        public AppDbContext() { }

        // DbContextOptions qəbul edən constructor - DI vasitəsilə real SQL Server
        // qoşulması ilə, və ya unit testlərdə SQLite in-memory kimi fərqli provider-lərlə
        // işə salına bilsin deyə əlavə olunub (bax: Tests/Managers/BookLoanTransactionTests.cs).
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // DI vasitəsilə (WebAPI-də AddDbContext ilə) və ya testlərdə (SQLite in-memory
            // üçün UseSqlite) artıq konfiqurasiya edilibsə, bu default SQL Server tənzimləməsi
            // TAMAMİLƏ ötürülür.
            if (optionsBuilder.IsConfigured)
                return;

            var configuration = BuildConfiguration();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection appsettings.json (və ya appsettings.{Environment}.json) " +
                    "faylında tapılmadı. Zəhmət olmasa konfiqurasiyanı yoxlayın. Qeyd: 'dotnet ef' əmrini " +
                    "birbaşa DataAccess layihəsindən işlədirsinizsə, --startup-project WebAPI parametrini " +
                    "əlavə edin ki, WebAPI-nin appsettings.json faylı istifadə olunsun.");
            }

            optionsBuilder.UseSqlServer(connectionString);
        }

        /// <summary>
        /// Parametrsiz konstruktorda (yuxarıda) DI-dan IConfiguration ala bilmədiyimiz üçün,
        /// konfiqurasiyanı burada özümüz quraşdırırıq. Prioritet ardıcıllığı ASP.NET Core-un
        /// öz WebApplicationBuilder-i ilə eynidir - beləliklə dev/prod arasında keçid appsettings
        /// faylını dəyişməklə və ya ASPNETCORE_ENVIRONMENT dəyişənini təyin etməklə mümkün olur,
        /// KODDA HEÇ BİR DƏYİŞİKLİK tələb olunmadan.
        /// </summary>
        private static IConfiguration BuildConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookLoan> BookLoans { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
