using Business.Abstract;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.BookLoanDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookLoanController : ControllerBase
    {
        private readonly IBookLoanService _bookLoanService;

        public BookLoanController(IBookLoanService bookLoanService)
        {
            _bookLoanService = bookLoanService;
        }

        /// <summary>
        /// Kitabı bir üzvə icarəyə verir. Bu əməliyyat İKİ cədvələ (Books - stok azalır,
        /// BookLoans - yeni qeyd yaranır) yazır və bunlar TAM BİR verilənlər bazası
        /// tranzaksiyası daxilində icra olunur: hər hansı addım uğursuz olarsa, bütün
        /// dəyişikliklər geri qaytarılır (rollback).
        /// </summary>
        /// <param name="dto">İcarəyə veriləcək kitab, üzv və müddət (gün).</param>
        /// <response code="201">Kitab uğurla icarəyə verildi.</response>
        /// <response code="400">Kitab stokda yoxdur.</response>
        /// <response code="404">Kitab və ya üzv tapılmadı.</response>
        [HttpPost("borrow")]
        [ProducesResponseType(typeof(IDataResult<GetBookLoanDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BorrowBook(BorrowBookDTO dto)
        {
            var result = await _bookLoanService.BorrowBookAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// İcarəyə verilmiş kitabı qaytarır. BookLoans.ReturnedAt təyin olunur və
        /// Books.Stock artırılır - hər ikisi eyni tranzaksiya daxilində.
        /// </summary>
        /// <param name="loanId">İcarə qeydinin identifikatoru.</param>
        /// <response code="200">Kitab uğurla qaytarıldı.</response>
        /// <response code="400">Bu kitab artıq qaytarılıb.</response>
        /// <response code="404">İcarə qeydi tapılmadı.</response>
        [HttpPost("return/{loanId}")]
        [ProducesResponseType(typeof(IDataResult<GetBookLoanDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReturnBook(Guid loanId)
        {
            var result = await _bookLoanService.ReturnBookAsync(loanId);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// ID-yə görə tək bir icarə qeydini qaytarır.
        /// </summary>
        /// <param name="id">İcarə qeydinin identifikatoru.</param>
        /// <response code="200">İcarə qeydi tapıldı.</response>
        /// <response code="404">Bu ID ilə icarə qeydi tapılmadı.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IDataResult<GetBookLoanDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLoanById(Guid id)
        {
            var result = await _bookLoanService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
