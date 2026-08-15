using AutoMapper;
using Business.Concrete;
using Business.Mapping;
using Core.Utilities.FileStorage;
using Core.Utilities.Pagination;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.BookDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text;
using Xunit;

namespace Tests.Managers
{
    public class BookManagerTests
    {
        private readonly Mock<IBookDAL> _bookDalMock;
        private readonly IMapper _mapper;
        private readonly BookManager _bookManager;
        private readonly Mock<IFileStorageService> _fileStorageMock;


        public BookManagerTests()
        {
            _bookDalMock = new Mock<IBookDAL>();
            _fileStorageMock = new Mock<IFileStorageService>();


            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<BookProfile>(), NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _bookManager = new BookManager(_bookDalMock.Object, _mapper, _fileStorageMock.Object);
        }

        private static IFormFile CreateFormFile(string fileName, string contentType, int sizeInBytes = 100)
        {
            var content = new byte[sizeInBytes];
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
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
        [Fact]
        public async Task GetByIdAsync_Should_Throw_When_Book_Not_Found()
        {
            var id = Guid.NewGuid();

            _bookDalMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((Book?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _bookManager.GetByIdAsync(id));
        }
        [Fact]
        public async Task DeleteAsync_Should_Throw_When_Book_Not_Found()
        {
            var id = Guid.NewGuid();

            _bookDalMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((Book?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _bookManager.DeleteAsync(id));

            _bookDalMock.Verify(x => x.DeleteAsync(It.IsAny<Book>()), Times.Never);
        }
        [Fact]
        public async Task GetAllBooksAsync_Should_Return_Filtered_Paged_Result()
        {
            var filter = new BookFilterParameters
            {
                PageNumber = 1,
                PageSize = 10,
                Title = "Clean",
                MinPrice = 10,
                MaxPrice = 100,
                InStockOnly = true
            };

            var books = new List<Book>
            {
                new Book { Id = Guid.NewGuid(), Title = "Clean Code", Price = 45, Stock = 5 }
            };

            _bookDalMock
                .Setup(x => x.GetAllAsync(filter))
                .ReturnsAsync((books, 1));

            var result = await _bookManager.GetAllBooksAsync(filter);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data!.Items);
            Assert.Equal(1, result.Data.TotalCount);
            Assert.Equal("Clean Code", result.Data.Items[0].Title);

            _bookDalMock.Verify(x => x.GetAllAsync(filter), Times.Once);
        }

        [Fact]
        public async Task FilterBooksAsync_Should_Return_Paged_Result_Using_Specification()
        {
            var filter = new BookFilterParameters
            {
                PageNumber = 1,
                PageSize = 10,
                AuthorName = "Martin",
                InStockOnly = true
            };

            var books = new List<Book>
                {
                    new Book { Id = Guid.NewGuid(), Title = "Clean Architecture", Price = 60, Stock = 2 }
                };

            _bookDalMock
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<Core.Specification.ISpecification<Book>>()))
                .ReturnsAsync((books, 1));

            var result = await _bookManager.FilterBooksAsync(filter);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data!.Items);
            Assert.Equal(1, result.Data.TotalCount);
            Assert.Equal("Clean Architecture", result.Data.Items[0].Title);

            _bookDalMock.Verify(
                x => x.GetBySpecificationAsync(It.IsAny<Core.Specification.ISpecification<Book>>()),
                Times.Once);
        }

        [Fact]
        public async Task SearchBooksNativeAsync_Should_Return_Books_From_Native_Query()
        {
            var books = new List<Book>
            {
                new Book { Id = Guid.NewGuid(), Title = "ASP.NET Core", Price = 50, Stock = 3 }
            };

            _bookDalMock
                .Setup(x => x.SearchBooksNativeAsync("asp", null, null, null))
                .ReturnsAsync(books);

            var result = await _bookManager.SearchBooksNativeAsync("asp", null, null, null);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data!);
            Assert.Equal("ASP.NET Core", result.Data![0].Title);

            _bookDalMock.Verify(x => x.SearchBooksNativeAsync("asp", null, null, null), Times.Once);
        }
        [Fact]
        public async Task UpdateAsync_Should_Throw_When_Book_Not_Found()
        {
            var id = Guid.NewGuid();

            var dto = new UpdateBookDTO
            {
                Title = "Doesn't Matter",
                Description = "N/A",
                Price = 10,
                Stock = 1,
                AuthorId = Guid.NewGuid()
            };

            _bookDalMock
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((Book?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _bookManager.UpdateAsync(id, dto));

            _bookDalMock.Verify(x => x.UpdateAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task UploadCoverImageAsync_Should_Save_File_And_Update_Book()
        {
            var id = Guid.NewGuid();
            var book = new Book { Id = id, Title = "Clean Code" };
            var file = CreateFormFile("cover.jpg", "image/jpeg");

            _bookDalMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(book);
            _fileStorageMock
                .Setup(x => x.SaveAsync(It.IsAny<Stream>(), $"{id}.jpg", "covers"))
                .ReturnsAsync($"covers/{id}.jpg");

            var result = await _bookManager.UploadCoverImageAsync(id, file);

            Assert.True(result.Success);
            Assert.Equal($"covers/{id}.jpg", book.CoverImagePath);
            _fileStorageMock.Verify(x => x.SaveAsync(It.IsAny<Stream>(), $"{id}.jpg", "covers"), Times.Once);
            _bookDalMock.Verify(x => x.UpdateAsync(book), Times.Once);
        }

        [Fact]
        public async Task UploadCoverImageAsync_Should_Throw_When_Book_Not_Found()
        {
            var id = Guid.NewGuid();
            var file = CreateFormFile("cover.jpg", "image/jpeg");

            _bookDalMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((Book?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _bookManager.UploadCoverImageAsync(id, file));
        }

        [Fact]
        public async Task UploadCoverImageAsync_Should_Throw_When_Extension_Not_Allowed()
        {
            var id = Guid.NewGuid();
            var book = new Book { Id = id, Title = "Clean Code" };
            var file = CreateFormFile("cover.gif", "image/gif");

            _bookDalMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(book);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _bookManager.UploadCoverImageAsync(id, file));

            _fileStorageMock.Verify(x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
        [Fact]
        public async Task UploadCoverImageAsync_Should_Throw_When_File_Too_Large()
        {
            var id = Guid.NewGuid();
            var book = new Book { Id = id, Title = "Clean Code" };
            var file = CreateFormFile("cover.jpg", "image/jpeg", sizeInBytes: 6 * 1024 * 1024);

            _bookDalMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(book);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _bookManager.UploadCoverImageAsync(id, file));

            _fileStorageMock.Verify(x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async Task DownloadCoverImageAsync_Should_Return_File_Content()
        {
            var id = Guid.NewGuid();
            var book = new Book { Id = id, Title = "Clean Code", CoverImagePath = "covers/" + id + ".png" };
            var bytes = Encoding.UTF8.GetBytes("fake-image-bytes");

            _bookDalMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(book);
            _fileStorageMock.Setup(x => x.ReadAsync(book.CoverImagePath)).ReturnsAsync(bytes);

            var result = await _bookManager.DownloadCoverImageAsync(id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(bytes, result.Data!.Content);
            Assert.Equal("image/png", result.Data.ContentType);
        }

        [Fact]
        public async Task DownloadCoverImageAsync_Should_Throw_When_Book_Has_No_Cover()
        {
            var id = Guid.NewGuid();
            var book = new Book { Id = id, Title = "Clean Code", CoverImagePath = null };

            _bookDalMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(book);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _bookManager.DownloadCoverImageAsync(id));
        }


    }
}
