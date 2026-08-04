using Entities.DTOs.CategoryDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validation.CategoryValidators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDTO>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MinimumLength(2).WithMessage("Category name must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Category name must be at most 100 characters long.");
        }
    }
}
