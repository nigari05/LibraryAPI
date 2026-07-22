using Entities.DTOs.MemberDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validation.MemberValidators
{
    public class CreateMemberValidator : AbstractValidator<CreateMemberDTO>
    {
        public CreateMemberValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Name must be at most 50 characters long.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Last name must be at most 50 characters long.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Please enter a valid email address.")
                .MaximumLength(100).WithMessage("Email must be at most 100 characters long.");
        }
    }
}
