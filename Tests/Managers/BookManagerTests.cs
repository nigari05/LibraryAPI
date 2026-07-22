using Business.Concrete;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.BookDTOs;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.Managers
{
    public class BookManagerTests
    {
        private readonly Mock<IBookDAL> _bookDalMock;
        private readonly BookManager _bookManager;

        public BookManagerTests()
        {
            _bookDalMock = new Mock<IBookDAL>();
            _bookManager = new BookManager(_bookDalMock.Object);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Book()
        {
       
            var dto = new CreateBookDTO
            {
                Title = "Clean Code",
                Description = "Programming Book",
                Price = 45,
                Stock = 20,
                AuthorId = Guid.NewGuid()
            };
            await _bookManager.AddAsync(dto);

            _bookDalMock.Verify(x => x.AddAsync(It.IsAny<Book>()), Times.Once);

        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Book()
        {
            Guid id = Guid.NewGuid();

            var book = new Book
            {
                Id = id,
                Title = "ASP.NET Core",
                Description = "Backend",
                Price = 50,
                Stock = 10
            };

            _bookDalMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(book);

            var result = await _bookManager.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(book.Title, result.Data.Title);
        }
        [Fact]
        public async Task DeleteAsync_Should_Delete_Book()
        {
            Guid id = Guid.NewGuid();

            var book = new Book
            {
                Id = id,
                Title = "Book"
            };

            _bookDalMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(book);

            await _bookManager.DeleteAsync(id);

            _bookDalMock.Verify(x => x.DeleteAsync(book), Times.Once);
        }
        [Fact]
        public async Task UpdateAsync_Should_Update_Book()
        {
            Guid id = Guid.NewGuid();

            var book = new Book
            {
                Id = id,
                Title = "Old Book",
                Price = 20,
                Stock = 5
            };

            var dto = new UpdateBookDTO
            {
                Title = "New Book",
                Description = "Updated",
                Price = 40,
                Stock = 10,
                AuthorId = Guid.NewGuid()
            };

            _bookDalMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(book);

            await _bookManager.UpdateAsync(id, dto);

            _bookDalMock.Verify(x => x.UpdateAsync(book), Times.Once);

            Assert.Equal(dto.Title, book.Title);
            Assert.Equal(dto.Price, book.Price);
        }
    }
}
