using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.BookLoanDTOs
{
    public class GetBookLoanDTO
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string? BookTitle { get; set; }
        public Guid MemberId { get; set; }
        public string? MemberFullName { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public bool IsReturned => ReturnedAt.HasValue;
    }
}
