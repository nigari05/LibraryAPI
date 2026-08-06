using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.BookLoanDTOs
{
    public class BorrowBookDTO
    {
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }

        /// <summary>İcarə müddəti (gün). Verilməzsə (0 və ya mənfi), 14 gün tətbiq olunur.</summary>
        public int LoanDays { get; set; } = 14;
    }
}
