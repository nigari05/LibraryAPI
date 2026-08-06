using Core.Utilities.Results.Abstract;
using Entities.DTOs.BookLoanDTOs;
using System;

namespace Business.Abstract
{
    public interface IBookLoanService
    {
        Task<IDataResult<GetBookLoanDTO>> BorrowBookAsync(BorrowBookDTO dto);

        Task<IDataResult<GetBookLoanDTO>> ReturnBookAsync(Guid loanId);

        Task<IDataResult<GetBookLoanDTO>> GetByIdAsync(Guid id);
    }
}
