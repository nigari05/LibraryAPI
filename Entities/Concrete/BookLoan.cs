using Entities.Concrete.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    /// <summary>
    /// Bir kitabın bir üzvə icarəyə verilməsini təmsil edir. Book (stok) və BookLoan
    /// (icarə qeydi) - hər bir borrow/return əməliyyatı bu İKİ cədvələ yazır və
    /// buna görə atomikliyi (@Transactional) tələb edir.
    /// </summary>
    public class BookLoan : BaseEntity
    {
        public Guid BookId { get; set; }
        public Book? Book { get; set; }

        public Guid MemberId { get; set; }
        public Member? Member { get; set; }

        public DateTime BorrowedAt { get; set; }
        public DateTime DueDate { get; set; }

        /// <summary>Kitab qaytarılana qədər null qalır.</summary>
        public DateTime? ReturnedAt { get; set; }
    }
}
