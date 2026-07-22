using Entities.DTOs.AuthorDTOS;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validation.AuthorValidators
{
    public class UpdateAuthorValidator : AbstractValidator<UpdateAuthorDTO>
    {
        public UpdateAuthorValidator()
        {
            RuleFor(x => x.FullName)
               .NotEmpty().WithMessage("Author name is required.")
               .MinimumLength(3).WithMessage("Author name must be at least 3 characters long.")
               .MaximumLength(100).WithMessage("Author name must be at most 100 characters long.");

            RuleFor(x => x.Biography)
                .MaximumLength(1000).WithMessage("Biography must be at most 1000 characters long.");
        }
    }
}
