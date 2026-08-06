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

        public BookLoanManager(IBookLoanDAL bookLoanDAL)
        {
            _bookLoanDAL = bookLoanDAL;
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

            var dtoResult = MapToDto(result!);
            return new SuccessDataResult<GetBookLoanDTO>(HttpStatusCode.Created, "Kitab uğurla icarəyə verildi.", dtoResult);
        }

        public async Task<IDataResult<GetBookLoanDTO>> ReturnBookAsync(Guid loanId)
        {
            await _bookLoanDAL.ReturnBookAsync(loanId);

            var result = await _bookLoanDAL.GetByIdAsync(loanId);

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
