using Business.Abstract;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.SuccessResults;
using DataAccess.Absract;
using Entities.DTOs.BookLoanDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;

namespace Business.Concrete
{
    public class BookLoanManager : IBookLoanService
    {
        private readonly IBookLoanDAL _bookLoanDAL;
        private readonly IEmailNotificationService _emailNotificationService;


        public BookLoanManager(IBookLoanDAL bookLoanDAL, IEmailNotificationService emailNotificationService)
        {
            _bookLoanDAL = bookLoanDAL;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<IDataResult<GetBookLoanDTO>> BorrowBookAsync(BorrowBookDTO dto)
        {
            Entities.Concrete.BookLoan createdLoan;
            try
            {
                createdLoan = await _bookLoanDAL.BorrowBookAsync(dto.BookId, dto.MemberId, dto.LoanDays);
            }
            catch (DbUpdateException)
            {
                // BookLoan sətrinin FK constraint-i (MemberId) pozulub - deməli üzv mövcud
                // deyil. Tranzaksiya artıq DAL qatında rollback olunub (Book.Stock də daxil
                // olmaqla), buna görə burada yalnız istifadəçiyə aydın mesaj qaytarırıq.
                throw new KeyNotFoundException("Üzv tapılmadı və ya əməliyyat uğursuz oldu.");
            }

            var result = await _bookLoanDAL.GetByIdAsync(createdLoan.Id);
            // Checkpoint 4 - Asinxron emal (@Async): email bildirişi növbəyə atılır və
            // BLOKLAMADAN davam edilir - HTTP cavabı email göndərilməsini gözləmir.
            if (result?.Member != null && !string.IsNullOrWhiteSpace(result.Member.Email))
            {
                await _emailNotificationService.QueueBorrowConfirmationAsync(
                    result.Member.Email,
                    $"{result.Member.FirstName} {result.Member.LastName}",
                    result.Book?.Title ?? string.Empty,
                    result.DueDate);
            }
            var dtoResult = MapToDto(result!);
            return new SuccessDataResult<GetBookLoanDTO>(HttpStatusCode.Created, "Kitab uğurla icarəyə verildi.", dtoResult);
        }

        public async Task<IDataResult<GetBookLoanDTO>> ReturnBookAsync(Guid loanId)
        {
            await _bookLoanDAL.ReturnBookAsync(loanId);

            var result = await _bookLoanDAL.GetByIdAsync(loanId);
            // Checkpoint 4 - Asinxron emal (@Async): eyni qeyri-bloklayan yanaşma.
            if (result?.Member != null && !string.IsNullOrWhiteSpace(result.Member.Email))
            {
                await _emailNotificationService.QueueReturnConfirmationAsync(
                    result.Member.Email,
                    $"{result.Member.FirstName} {result.Member.LastName}",
                    result.Book?.Title ?? string.Empty);
            }
            var dtoResult = MapToDto(result!);
            return new SuccessDataResult<GetBookLoanDTO>(HttpStatusCode.OK, "Kitab uğurla qaytarıldı.", dtoResult);
        }

        public async Task<IDataResult<GetBookLoanDTO>> GetByIdAsync(Guid id)
        {
            var loan = await _bookLoanDAL.GetByIdAsync(id);

            if (loan == null)
                throw new KeyNotFoundException("İcarə qeydi tapılmadı.");

            return new SuccessDataResult<GetBookLoanDTO>(HttpStatusCode.OK, MapToDto(loan));
        }

        private static GetBookLoanDTO MapToDto(Entities.Concrete.BookLoan loan)
        {
            return new GetBookLoanDTO
            {
                Id = loan.Id,
                BookId = loan.BookId,
                BookTitle = loan.Book?.Title ?? string.Empty,
                MemberId = loan.MemberId,
                MemberFullName = loan.Member != null ? $"{loan.Member.FirstName} {loan.Member.LastName}" : string.Empty,
                BorrowedAt = loan.BorrowedAt,
                DueDate = loan.DueDate,
                ReturnedAt = loan.ReturnedAt
            };
        }
    }
}
