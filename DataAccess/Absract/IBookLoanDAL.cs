using Entities.Concrete;
using System;

namespace DataAccess.Absract
{
    /// <summary>
    /// Kitab icarəsi (borrow/return) üçün DAL kontraktı. BorrowBookAsync və
    /// ReturnBookAsync hər ikisi bir neçə cədvələ (Books + BookLoans) yazır və
    /// tam bir verilənlər bazası tranzaksiyası daxilində icra olunur.
    /// </summary>
    public interface IBookLoanDAL
    {
        Task<BookLoan?> GetByIdAsync(Guid id);

        /// <summary>
        /// Book.Stock-u 1 azaldır və yeni BookLoan sətri yaradır - EYNİ tranzaksiya
        /// daxilində. Hər hansı addım uğursuz olarsa, hər iki dəyişiklik geri qaytarılır.
        /// </summary>
        Task<BookLoan> BorrowBookAsync(Guid bookId, Guid memberId, int loanDays);

        /// <summary>
        /// BookLoan.ReturnedAt-i təyin edir və Book.Stock-u 1 artırır - EYNİ tranzaksiya
        /// daxilində.
        /// </summary>
        Task<BookLoan> ReturnBookAsync(Guid loanId);

        /// <summary>
        /// Müddəti keçmiş (DueDate keçib) və hələ qaytarılmamış (ReturnedAt == null)
        /// bütün icarələri qaytarır. Planlaşdırılmış gündəlik təmizləmə tapşırığı
        /// tərəfindən istifadə olunur.
        /// </summary>
        Task<List<BookLoan>> GetOverdueLoansAsync();
    }
}
