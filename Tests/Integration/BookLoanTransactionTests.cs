using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.Integration
{
    /// <summary>
    /// EfBookLoanDAL.BorrowBookAsync/ReturnBookAsync-in real ACID tranzaksiya davranışını
    /// yoxlayan testlər. Mock deyil, real SQLite in-memory verilənlər bazası istifadə olunur
    /// (":memory:" + FK constraint-lər aktiv), çünki EF Core-un standart InMemory provider-i
    /// həqiqi tranzaksiya/rollback semantikasını dəstəkləmir - SQLite isə dəstəkləyir.
    /// </summary>
    public class BookLoanTransactionTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _options;

        public BookLoanTransactionTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new AppDbContext(_options);
            context.Database.EnsureCreated();
        }

        public void Dispose() => _connection.Dispose();

        private async Task<(Guid BookId, Guid MemberId)> SeedBookAndMemberAsync(int stock = 3)
        {
            await using var context = new AppDbContext(_options);

            var author = new Author { Id = Guid.NewGuid(), FullName = "Robert C. Martin" };
            var book = new Book { Id = Guid.NewGuid(), Title = "Clean Code", Price = 45, Stock = stock, AuthorId = author.Id };
            var member = new Member { Id = Guid.NewGuid(), FirstName = "Nigari", LastName = "Zulfugarova", Email = "nigari@example.com" };

            context.Authors.Add(author);
            context.Books.Add(book);
            context.Members.Add(member);
            await context.SaveChangesAsync();

            return (book.Id, member.Id);
        }

        [Fact]
        public async Task BorrowBookAsync_Should_Commit_Both_Writes_On_Success()
        {
            var (bookId, memberId) = await SeedBookAndMemberAsync(stock: 3);

            await using var context = new AppDbContext(_options);
            var dal = new EfBookLoanDAL(context);

            var loan = await dal.BorrowBookAsync(bookId, memberId, loanDays: 14);

            await using var verifyContext = new AppDbContext(_options);
            var book = await verifyContext.Books.FindAsync(bookId);
            var loanCount = await verifyContext.BookLoans.CountAsync();

            Assert.Equal(2, book!.Stock);
            Assert.Equal(1, loanCount);
            Assert.Null(loan.ReturnedAt);
        }

        [Fact]
        public async Task BorrowBookAsync_Should_Rollback_Stock_Decrement_When_Member_Does_Not_Exist()
        {
            var (bookId, _) = await SeedBookAndMemberAsync(stock: 3);
            var nonExistentMemberId = Guid.NewGuid(); // heç bir Member sətri yaradılmayıb

            await using var context = new AppDbContext(_options);
            var dal = new EfBookLoanDAL(context);

            // BookLoan sətrinin MemberId FK constraint-i pozulacaq -> SaveChangesAsync
            // (2-ci yazı) DbUpdateException atacaq -> catch bloku transaction.RollbackAsync()
            // çağıracaq -> Book.Stock-un 1-ci yazıda edilən azalması da geri qaytarılmalıdır.
            await Assert.ThrowsAnyAsync<DbUpdateException>(
                () => dal.BorrowBookAsync(bookId, nonExistentMemberId, loanDays: 14));

            await using var verifyContext = new AppDbContext(_options);
            var book = await verifyContext.Books.FindAsync(bookId);
            var loanCount = await verifyContext.BookLoans.CountAsync();

            Assert.Equal(3, book!.Stock); // rollback sayəsində dəyişməyib
            Assert.Equal(0, loanCount);   // heç bir yarımçıq BookLoan sətri qalmayıb
        }

        [Fact]
        public async Task ReturnBookAsync_Should_Increment_Stock_And_Set_ReturnedAt_On_Success()
        {
            var (bookId, memberId) = await SeedBookAndMemberAsync(stock: 3);

            await using var borrowContext = new AppDbContext(_options);
            var borrowDal = new EfBookLoanDAL(borrowContext);
            var loan = await borrowDal.BorrowBookAsync(bookId, memberId, loanDays: 14);

            await using var returnContext = new AppDbContext(_options);
            var returnDal = new EfBookLoanDAL(returnContext);
            var returnedLoan = await returnDal.ReturnBookAsync(loan.Id);

            await using var verifyContext = new AppDbContext(_options);
            var book = await verifyContext.Books.FindAsync(bookId);

            Assert.Equal(3, book!.Stock); // borrow (-1) + return (+1) = ilkin dəyər
            Assert.NotNull(returnedLoan.ReturnedAt);
        }

        [Fact]
        public async Task ReturnBookAsync_Should_Throw_When_Loan_Already_Returned()
        {
            var (bookId, memberId) = await SeedBookAndMemberAsync(stock: 3);

            await using var borrowContext = new AppDbContext(_options);
            var borrowDal = new EfBookLoanDAL(borrowContext);
            var loan = await borrowDal.BorrowBookAsync(bookId, memberId, loanDays: 14);

            await using var firstReturnContext = new AppDbContext(_options);
            await new EfBookLoanDAL(firstReturnContext).ReturnBookAsync(loan.Id);

            await using var secondReturnContext = new AppDbContext(_options);
            var secondReturnDal = new EfBookLoanDAL(secondReturnContext);

            await Assert.ThrowsAsync<ArgumentException>(() => secondReturnDal.ReturnBookAsync(loan.Id));

            // Stok ikinci dəfə artırılmamalıdır (3 - 1 + 1 = 3, ikinci cəhd rədd edilib)
            await using var verifyContext = new AppDbContext(_options);
            var book = await verifyContext.Books.FindAsync(bookId);
            Assert.Equal(3, book!.Stock);
        }
    }
}
