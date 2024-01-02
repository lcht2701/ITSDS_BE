using API.DTOs.Requests.Users;
using Domain.Constants.Enums;
using Domain.Models;
using FluentValidation;
using Persistence.Repositories.Interfaces;

namespace API.Validations.Users
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
    {
        private readonly IRepositoryBase<User> _userRepository;
        public UpdateUserValidator(IRepositoryBase<User> userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name should not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name should not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .UniqueEmail(email => _userRepository.FirstOrDefaultAsync(x => x.Email.Equals(email)).Result != null).WithMessage("Email Address is already in use.")
                .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(u => u.Role)
                .IsInEnum()
                .Must(role => role >= Role.Admin && role <= Role.Accountant)
                .WithMessage("Role must be valid and is required.");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive is required.");

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


