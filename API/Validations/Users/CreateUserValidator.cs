using API.DTOs.Requests.Users;
using Domain.Constants.Enums;
using Domain.Models;
using FluentValidation;
using Persistence.Repositories.Interfaces;

namespace API.Validations.Users;

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    private readonly IRepositoryBase<User> _userRepository;
    public CreateUserValidator(IRepositoryBase<User> userRepository)
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

        RuleFor(u => u.Email)
            .EmailAddress().WithMessage("Email Address is invalid")
            .Must(email => IsUniqueEmail(email)).WithMessage("Email Address is already in use.")
            .NotEmpty().WithMessage("Email Address is required.");

        RuleFor(u => u.Role)
            .IsInEnum()
            .Must(role => role >= Role.Admin && role <= Role.Accountant)
            .WithMessage("Role must be valid and is required.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value.")
            .Must(gender => gender >= Gender.Male && gender <= Gender.Female)
            .WithMessage("Role must be valid and is required.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(15)
            .WithMessage("Phone number should not exceed 15 characters.")
            .Matches(@"^\+?[0-9-]*$").WithMessage("Invalid phone number format.");
    }

    private bool IsUniqueEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false; // or true, depending on your business logic
        }

        var user = _userRepository.FirstOrDefaultAsync(x => x.Email.Equals(email)).Result;
        return user == null;
    }
}