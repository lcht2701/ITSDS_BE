using API.DTOs.Requests.CompanyMembers;
using Domain.Constants.Enums;
using Domain.Models;
using FluentValidation;
using Persistence.Repositories.Interfaces;

namespace API.Validations.CompanyMembers
{
    public class AddAccountInformationValidator : AbstractValidator<AddAccountInformationRequest>
    {
        private readonly IRepositoryBase<User> _userRepository;

        public AddAccountInformationValidator(IRepositoryBase<User> userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name should not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name should not exceed 50 characters.");

            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

            RuleFor(u => u.Email)
                .EmailAddress().WithMessage("Email Address is invalid")
                .UniqueEmail(email => _userRepository.FirstOrDefaultAsync(x => x.Email.Equals(email)).Result == null).WithMessage("Email Address is already in use.")
                .NotEmpty().WithMessage("Email Address is required.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value.")
                .Must(gender => gender >= Gender.Male && gender <= Gender.PreferNotToSay)
                .WithMessage("Role must be valid and is required.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).When(x => x.DateOfBirth != null)
                .WithMessage("Date of birth should be in the past.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).When(x => x.DateOfBirth != null)
                .WithMessage("Phone number should not exceed 15 characters.")
                .Matches(@"^\+?[0-9-]*$").WithMessage("Invalid phone number format.");
        }

        private bool BeUniqueEmail(string email)
        {
            // Implement the logic to check if the email is unique in your service or repository
            return _userRepository.FirstOrDefaultAsync(x => x.Email.Equals(email)).Result == null;
        }
    }
}
