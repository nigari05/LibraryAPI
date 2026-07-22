using Entities.DTOs.BookDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validation.BookValidators
{
    public class UpdateBookValidator : AbstractValidator<UpdateBookDTO>
    {
        public UpdateBookValidator()
        {
            RuleFor(x => x.Title)
                  .NotEmpty().WithMessage("Book title is required.")
                  .MinimumLength(2).WithMessage("Book title must be at least 2 characters long.")
                  .MaximumLength(200).WithMessage("Book title must be at most 200 characters long.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must be at most 1000 characters long.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be a positive value.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("Author must be selected.");

        }
}
