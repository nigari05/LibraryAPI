using Business.Abstract;
using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Results.Concrete.SuccessResults;
using DataAccess.Absract;
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
        private readonly IBookDAL _bookDAL;

        public BookManager(IBookDAL bookDAL)
        {
            _bookDAL = bookDAL;
        }

        public async Task<IResult> AddAsync(CreateBookDTO entity)
        {
            var book = new Book
            {
                Title = entity.Title,
                Description = entity.Description,
                Price = entity.Price,
                Stock = entity.Stock,
                AuthorId = entity.AuthorId
            };

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

        

        public async Task<IDataResult<List<GetBookDTO>>> GetAllBooksAsync(PaginationParameters pagination )
        {
            var books = await _bookDAL.GetAllAsync(pagination);

            List<GetBookDTO> bookDTOs =  books.Select(book => new GetBookDTO  
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                Stock = book.Stock,
                AuthorName = book.Author != null ? book.Author.FullName : string.Empty
            }).ToList();

            return new SuccessDataResult<List<GetBookDTO>>( HttpStatusCode.OK, bookDTOs);
        }

        public async Task<IDataResult<GetBookDTO?>> GetByIdAsync(Guid id)
        {
            var book = await _bookDAL.GetByIdAsync(id);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");


            GetBookDTO module=  new()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                Stock = book.Stock,
                AuthorName = book.Author != null ? book.Author.FullName : string.Empty
            };
            return new SuccessDataResult<GetBookDTO?>(HttpStatusCode.OK, module);
        }

        public async Task<IResult> UpdateAsync(Guid id, UpdateBookDTO entity)
        {
            var book = await _bookDAL.GetByIdAsync(id);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            book.Title = entity.Title;
            book.Description = entity.Description;
            book.Price = entity.Price;
            book.Stock = entity.Stock;
            book.AuthorId = entity.AuthorId;

            await _bookDAL.UpdateAsync(book);
            return new SuccessResult(HttpStatusCode.NoContent, "Book updated successfully.");
        }
    }
}
