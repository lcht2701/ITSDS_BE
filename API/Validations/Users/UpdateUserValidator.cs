using API.DTOs.Requests.Users;
using FluentValidation;

namespace API.Validations.Users
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name should not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name should not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address format.")
                .MaximumLength(100).WithMessage("Email should not exceed 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).When(x => x.DateOfBirth != null)
                .WithMessage("Phone number should not exceed 15 characters.")
                .Matches(@"^\+?[0-9-]*$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value.");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive is required.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Now).When(x => x.DateOfBirth != null)
                .WithMessage("Date of birth should be in the past.");

            RuleFor(x => x.Address)
                .MaximumLength(200).When(x => x.DateOfBirth != null)
                .WithMessage("Address should not exceed 200 characters.");
        }
    }
}


