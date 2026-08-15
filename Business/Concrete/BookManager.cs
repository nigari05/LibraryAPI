using AutoMapper;
using Business.Abstract;
using Core.Utilities.FileStorage;
using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Results.Concrete.SuccessResults;
using DataAccess.Absract;
using DataAccess.Specification;
using Entities.Concrete;
using Entities.DTOs.BookDTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Business.Concrete
{
    public class BookManager : IBookService
    {
        private const long MaxCoverImageSizeBytes = 5 * 1024 * 1024; // 5 MB
        private const string CoverImagesSubFolder = "covers";
        private readonly IBookDAL _bookDAL;
        private readonly  IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public BookManager(IBookDAL bookDAL, IMapper mapper, IFileStorageService fileStorageService)
        {
            _bookDAL = bookDAL;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        public async Task<IResult> AddAsync(CreateBookDTO entity)
        {
            var book = _mapper.Map<Book>(entity);
            await _bookDAL.AddAsync(book);
            return new SuccessResult(HttpStatusCode.Created, "Book added successfully.");
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var book = await _bookDAL.GetByIdAsync(id);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            await _bookDAL.DeleteAsync(book);
            return new SuccessResult(HttpStatusCode.NoContent, "Book deleted successfully.");
        }

        public async Task<IDataResult<BookCoverDTO>> DownloadCoverImageAsync(Guid id)
        {
            var book = await _bookDAL.GetByIdAsync(id);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            if (string.IsNullOrEmpty(book.CoverImagePath))
                throw new KeyNotFoundException("Bu kitab üçün üz qabığı şəkli yüklənməyib.");

            var content = await _fileStorageService.ReadAsync(book.CoverImagePath);

            if (content == null)
                throw new KeyNotFoundException("Şəkil fayl sistemində tapılmadı.");

            var coverDTO = new BookCoverDTO
            {
                Content = content,
                ContentType = FileContentTypeResolver.Resolve(book.CoverImagePath),
                FileName = Path.GetFileName(book.CoverImagePath)
            };

            return new SuccessDataResult<BookCoverDTO>(HttpStatusCode.OK, coverDTO);
        }

        public async Task<IDataResult<PagedResult<GetBookDTO>>> FilterBooksAsync(BookFilterParameters filterParameters)
        {
            var specification = new BookFilterSpecification(filterParameters);

            var (books, totalCount) = await _bookDAL.GetBySpecificationAsync(specification);

            var bookDTOs = _mapper.Map<List<GetBookDTO>>(books);

            var result = new PagedResult<GetBookDTO>
            {
                Items = bookDTOs,
                TotalCount = totalCount,
                CurrentPage = filterParameters.PageNumber,
                PageSize = filterParameters.PageSize
            };

            return new SuccessDataResult<PagedResult<GetBookDTO>>(HttpStatusCode.OK, result);
        }

        public async Task<IDataResult<PagedResult<GetBookDTO>>> GetAllBooksAsync(BookFilterParameters filterParameters)
        {
            var (books, totalCount) = await _bookDAL.GetAllAsync(filterParameters);

            var bookDTOs = _mapper.Map<List<GetBookDTO>>(books);

            var result = new PagedResult<GetBookDTO>
            {
                Items = bookDTOs,
                TotalCount = totalCount,
                CurrentPage = filterParameters.PageNumber,
                PageSize = filterParameters.PageSize
            };

            return new SuccessDataResult<PagedResult<GetBookDTO>>(HttpStatusCode.OK, result);
        }

        public async Task<IDataResult<GetBookDTO?>> GetByIdAsync(Guid id)
        {
            
            // N+1 fix: əvvəllər _bookDAL.GetByIdAsync (generic FindAsync) Author/Categories
            // əlaqələrini yükləmirdi - AuthorName həmişə boş qalırdı. İndi Include ilə TƏK
            // sorğuda hamısı gətirilir.
            var book = await _bookDAL.GetByIdWithDetailsAsync(id);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");


            GetBookDTO module = new()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                Stock = book.Stock,
                AuthorName = book.Author != null ? book.Author.FullName : string.Empty,
                CategoryNames = book.Categories?.Select(c => c.Name).ToList() ?? new List<string>()
            };
            return new SuccessDataResult<GetBookDTO?>(HttpStatusCode.OK, module);
        }

        public async Task<IDataResult<List<GetBookDTO>>> SearchBooksNativeAsync(string? keyword, decimal? minPrice, decimal? maxPrice, Guid? categoryId)
        {
            var books = await _bookDAL.SearchBooksNativeAsync(keyword, minPrice, maxPrice, categoryId);

            var bookDTOs = _mapper.Map<List<GetBookDTO>>(books);

            return new SuccessDataResult<List<GetBookDTO>>(HttpStatusCode.OK, bookDTOs);
        }

        public async Task<IResult> UpdateAsync(Guid id, UpdateBookDTO entity)
        {
            var book = await _bookDAL.GetByIdAsync(id);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            _mapper.Map(entity, book);

            await _bookDAL.UpdateAsync(book);
            return new SuccessResult(HttpStatusCode.NoContent, "Book updated successfully.");
        }

        public async Task<IResult> UploadCoverImageAsync(Guid id, IFormFile file)
        {
            var book = await _bookDAL.GetByIdAsync(id);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            if (file == null || file.Length == 0)
                throw new ArgumentException("Fayl seçilməyib.");

            if (file.Length > MaxCoverImageSizeBytes)
                throw new ArgumentException($"Fayl ölçüsü {MaxCoverImageSizeBytes / (1024 * 1024)} MB-dan böyük ola bilməz.");

            var extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(extension) || !FileContentTypeResolver.IsAllowedExtension(extension))
                throw new ArgumentException("Yalnız .jpg, .jpeg, .png, .webp formatlı fayllar qəbul edilir.");

            if (string.IsNullOrWhiteSpace(file.ContentType) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Göndərilən fayl şəkil formatında deyil.");

            // Köhnə şəkil fərqli uzantı ilə yüklənmişdisə, disk üzərində sürünüb qalmasın.
            if (!string.IsNullOrEmpty(book.CoverImagePath))
                _fileStorageService.Delete(book.CoverImagePath);

            var fileName = $"{book.Id}{extension}";

            await using (var stream = file.OpenReadStream())
            {
                book.CoverImagePath = await _fileStorageService.SaveAsync(stream, fileName, CoverImagesSubFolder);
            }

            await _bookDAL.UpdateAsync(book);

            return new SuccessResult(HttpStatusCode.OK, "Kitabın üz qabığı şəkli uğurla yükləndi.");
        }
    }
}
