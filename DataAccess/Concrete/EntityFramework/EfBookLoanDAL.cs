using DataAccess.Absract;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DataAccess.Concrete.EntityFramework
{
    /// <summary>
    /// DİQQƏT: Bu sinif EfRepositorybase-dən İRSƏN GƏLMİR (unlike EfBookDAL). Səbəb:
    /// EfRepositorybase-in hər metodu ("using TContext context = new();") üçün AYRI bir
    /// DbContext yaradır və dərhal SaveChangesAsync çağırır - yəni hər çağırış öz-özlüyündə
    /// artıq "commit" olunur. Bu, iki AYRI repository çağırışını (məs. Book.Update + BookLoan.Add)
    /// bir ORTAQ tranzaksiyada birləşdirməyi mümkünsüz edir. Ona görə bu DAL, DI vasitəsilə
    /// inject olunan EYNİ (scoped) AppDbContext üzərində işləyir və explicit tranzaksiya
    /// idarə edir - Java-dakı @Transactional metodunun .NET/EF Core ekvivalenti.
    /// </summary>
    public class EfBookLoanDAL : IBookLoanDAL
    {
        private readonly AppDbContext _context;

        public EfBookLoanDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookLoan?> GetByIdAsync(Guid id)
        {
            return await _context.BookLoans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<BookLoan> BorrowBookAsync(Guid bookId, Guid memberId, int loanDays)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId)
                ?? throw new KeyNotFoundException("Kitab tapılmadı.");

            if (book.Stock <= 0)
                throw new ArgumentException("Kitab hazırda stokda yoxdur.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1-ci yazı: Books cədvəli (stok 1 azaldılır)
                book.Stock -= 1;
                await _context.SaveChangesAsync();

                // 2-ci yazı: BookLoans cədvəli. MemberId-nin mövcudluğu verilənlər bazası
                // FK constraint-i ilə təmin olunur - üzv yoxdursa bu SaveChangesAsync
                // DbUpdateException atır və catch bloku YUXARIDAKI stok azalmasını da
                // geri qaytarır (rollback) - beləliklə heç vaxt "stok azalıb, amma icarə
                // qeydi yaranmayıb" kimi qeyri-ardıcıl vəziyyət yaranmır.
                var loan = new BookLoan
                {
                    Id = Guid.NewGuid(),
                    BookId = bookId,
                    MemberId = memberId,
                    BorrowedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(loanDays > 0 ? loanDays : 14)
                };

                _context.BookLoans.Add(loan);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return loan;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<BookLoan> ReturnBookAsync(Guid loanId)
        {
            var loan = await _context.BookLoans.FirstOrDefaultAsync(l => l.Id == loanId)
                ?? throw new KeyNotFoundException("İcarə qeydi tapılmadı.");

            if (loan.ReturnedAt.HasValue)
                throw new ArgumentException("Bu kitab artıq qaytarılıb.");

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == loan.BookId)
                ?? throw new KeyNotFoundException("Kitab tapılmadı.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1-ci yazı: BookLoans cədvəli
                loan.ReturnedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // 2-ci yazı: Books cədvəli
                book.Stock += 1;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return loan;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
