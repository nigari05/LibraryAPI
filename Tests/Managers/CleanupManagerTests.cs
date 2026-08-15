using Business.Concrete;
using Core.Utilities.FileStorage;
using DataAccess.Absract;
using Entities.Concrete;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.Managers
{
    public class CleanupManagerTests
    {
        private readonly Mock<IBookDAL> _bookDalMock;
        private readonly Mock<IBookLoanDAL> _bookLoanDalMock;
        private readonly Mock<IFileStorageService> _fileStorageMock;
        private readonly CleanupManager _cleanupManager;

        public CleanupManagerTests()
        {
            _bookDalMock = new Mock<IBookDAL>();
            _bookLoanDalMock = new Mock<IBookLoanDAL>();
            _fileStorageMock = new Mock<IFileStorageService>();

            _cleanupManager = new CleanupManager(_bookDalMock.Object, _bookLoanDalMock.Object, _fileStorageMock.Object);
        }

        [Fact]
        public async Task RunDailyCleanupAsync_Should_Remove_Only_Orphaned_Files()
        {
            var bookId = Guid.NewGuid();

            _bookDalMock
                .Setup(x => x.GetAllCoverImagePathsAsync())
                .ReturnsAsync(new List<string> { $"covers/{bookId}.jpg" });

            _fileStorageMock
                .Setup(x => x.ListFiles("covers"))
                .Returns(new[] { $"covers/{bookId}.jpg", "covers/orphaned-file.png" });

            _bookLoanDalMock
                .Setup(x => x.GetOverdueLoansAsync())
                .ReturnsAsync(new List<BookLoan>());

            var result = await _cleanupManager.RunDailyCleanupAsync();

            Assert.Equal(1, result.OrphanedCoverImagesRemoved);
            _fileStorageMock.Verify(x => x.Delete("covers/orphaned-file.png"), Times.Once);
            _fileStorageMock.Verify(x => x.Delete($"covers/{bookId}.jpg"), Times.Never);
        }

        [Fact]
        public async Task RunDailyCleanupAsync_Should_Report_Overdue_Loan_Count()
        {
            _bookDalMock
                .Setup(x => x.GetAllCoverImagePathsAsync())
                .ReturnsAsync(new List<string>());

            _fileStorageMock
                .Setup(x => x.ListFiles("covers"))
                .Returns(Enumerable.Empty<string>());

            var overdueLoans = new List<BookLoan>
            {
                new BookLoan { Id = Guid.NewGuid(), DueDate = DateTime.UtcNow.AddDays(-3) },
                new BookLoan { Id = Guid.NewGuid(), DueDate = DateTime.UtcNow.AddDays(-1) }
            };

            _bookLoanDalMock
                .Setup(x => x.GetOverdueLoansAsync())
                .ReturnsAsync(overdueLoans);

            var result = await _cleanupManager.RunDailyCleanupAsync();

            Assert.Equal(2, result.OverdueLoansFound);
            Assert.Equal(0, result.OrphanedCoverImagesRemoved);
        }

        [Fact]
        public async Task RunDailyCleanupAsync_Should_Not_Delete_Anything_When_No_Files_Exist()
        {
            _bookDalMock
                .Setup(x => x.GetAllCoverImagePathsAsync())
                .ReturnsAsync(new List<string>());

            _fileStorageMock
                .Setup(x => x.ListFiles("covers"))
                .Returns(Enumerable.Empty<string>());

            _bookLoanDalMock
                .Setup(x => x.GetOverdueLoansAsync())
                .ReturnsAsync(new List<BookLoan>());

            var result = await _cleanupManager.RunDailyCleanupAsync();

            Assert.Equal(0, result.OrphanedCoverImagesRemoved);
            Assert.Equal(0, result.OverdueLoansFound);
            _fileStorageMock.Verify(x => x.Delete(It.IsAny<string>()), Times.Never);
        }
    }
}
