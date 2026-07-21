using Business.Abstract;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.BookDTOs;
using System;
using System.Collections.Generic;
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

        public async Task AddAsync(CreateBookDTO entity)
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
        }

        public async Task DeleteAsync(Guid id)
        {
            var book = await _bookDAL.GetByIdAsync(id);

            if (book == null)
                throw new Exception("Book not found.");

            await _bookDAL.DeleteAsync(book);
        }

        public async Task<List<GetBookDTO>> GetAllBooksAsync()
        {
            var books = await _bookDAL.GetAllAsync();

            return books.Select(book => new GetBookDTO  
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                Stock = book.Stock,
                AuthorName = book.Author != null ? book.Author.FullName : string.Empty
            }).ToList();
        }

        public async Task<GetBookDTO?> GetByIdAsync(Guid id)
        {
            var book = await _bookDAL.GetByIdAsync(id);

            if (book == null)
                return null;

            return new GetBookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                Stock = book.Stock,
                AuthorName = book.Author != null ? book.Author.FullName : string.Empty
            };
        }

        public async Task UpdateAsync(Guid id, UpdateBookDTO entity)
        {
            var book = await _bookDAL.GetByIdAsync(id);

            if (book == null)
                throw new Exception("Book not found.");

            book.Title = entity.Title;
            book.Description = entity.Description;
            book.Price = entity.Price;
            book.Stock = entity.Stock;
            book.AuthorId = entity.AuthorId;

            await _bookDAL.UpdateAsync(book);
        }
    }
}
