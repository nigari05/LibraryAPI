using AutoMapper;
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
        private readonly  IMapper _mapper;

        public BookManager(IBookDAL bookDAL, IMapper mapper)
        {
            _bookDAL = bookDAL;
            _mapper = mapper;
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

            _mapper.Map(entity, book);

            await _bookDAL.UpdateAsync(book);
            return new SuccessResult(HttpStatusCode.NoContent, "Book updated successfully.");
        }

        
       
    }
}
