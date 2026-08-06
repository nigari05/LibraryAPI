using Entities.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
            optionsBuilder.UseSqlServer("Server=localhost;Database=LibraryDb;Trusted_Connection=true; trustServerCertificate=true;");
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
