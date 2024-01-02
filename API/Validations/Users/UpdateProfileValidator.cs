using API.DTOs.Requests.Users;
using FluentValidation;

namespace API.Validations.Users
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidator()
        {
            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("First Name is required");

            RuleFor(u => u.LastName)
                .NotEmpty().WithMessage("Last Name is required");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email Address is required")
                .EmailAddress().WithMessage("Email Address is invalid.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).When(x => x.PhoneNumber != null)
                .WithMessage("Phone number should not exceed 15 characters.")
                .Matches(@"^\+?[0-9-]*$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).When(x => x.DateOfBirth != null)
                .WithMessage("Date of birth should be in the past.");
        }
    }
}


