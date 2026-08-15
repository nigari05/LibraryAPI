using Business.Abstract;
using Business.Concrete;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.BookLoanDTOs;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.Managers
{
    public class BookLoanManagerTests
    {
        private readonly Mock<IBookLoanDAL> _bookLoanDalMock;
        private readonly Mock<IEmailNotificationService> _emailNotificationServiceMock;
        private readonly BookLoanManager _bookLoanManager;

        public BookLoanManagerTests()
        {
            _bookLoanDalMock = new Mock<IBookLoanDAL>();
            _emailNotificationServiceMock = new Mock<IEmailNotificationService>();

            _bookLoanManager = new BookLoanManager(_bookLoanDalMock.Object, _emailNotificationServiceMock.Object);
        }

        private static BookLoan CreateLoanWithNavigations(Guid loanId, Guid bookId, Guid memberId, string? memberEmail = "member@example.com")
        {
            return new BookLoan
            {
                Id = loanId,
                BookId = bookId,
                MemberId = memberId,
                BorrowedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                Book = new Book { Id = bookId, Title = "Clean Code" },
                Member = memberEmail == null
                    ? null
                    : new Member { Id = memberId, FirstName = "Nigari", LastName = "Zulfuqarova", Email = memberEmail }
            };
        }

        [Fact]
        public async Task BorrowBookAsync_Should_Queue_Confirmation_Email_When_Member_Has_Email()
        {
            var loanId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var dto = new BorrowBookDTO { BookId = bookId, MemberId = memberId, LoanDays = 14 };
            var loan = CreateLoanWithNavigations(loanId, bookId, memberId);

            _bookLoanDalMock
                .Setup(x => x.BorrowBookAsync(bookId, memberId, dto.LoanDays))
                .ReturnsAsync(loan);

            _bookLoanDalMock
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync(loan);

            var result = await _bookLoanManager.BorrowBookAsync(dto);

            Assert.True(result.Success);
            _emailNotificationServiceMock.Verify(
                x => x.QueueBorrowConfirmationAsync("member@example.com", "Nigari Zulfuqarova", "Clean Code", loan.DueDate),
                Times.Once);
        }

        [Fact]
        public async Task BorrowBookAsync_Should_Not_Queue_Email_When_Member_Has_No_Email()
        {
            var loanId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var dto = new BorrowBookDTO { BookId = bookId, MemberId = memberId, LoanDays = 14 };
            var loan = CreateLoanWithNavigations(loanId, bookId, memberId, memberEmail: null);

            _bookLoanDalMock
                .Setup(x => x.BorrowBookAsync(bookId, memberId, dto.LoanDays))
                .ReturnsAsync(loan);

            _bookLoanDalMock
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync(loan);

            await _bookLoanManager.BorrowBookAsync(dto);

            _emailNotificationServiceMock.Verify(
                x => x.QueueBorrowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);
        }

        [Fact]
        public async Task ReturnBookAsync_Should_Queue_Confirmation_Email()
        {
            var loanId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var loan = CreateLoanWithNavigations(loanId, bookId, memberId);
            loan.ReturnedAt = DateTime.UtcNow;

            _bookLoanDalMock
                .Setup(x => x.ReturnBookAsync(loanId))
                .ReturnsAsync(loan);

            _bookLoanDalMock
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync(loan);    

            var result = await _bookLoanManager.ReturnBookAsync(loanId);

            Assert.True(result.Success);
            _emailNotificationServiceMock.Verify(
                x => x.QueueReturnConfirmationAsync("member@example.com", "Nigari Zulfuqarova", "Clean Code"),
                Times.Once);
        }
    }
}
