using API.DTOs.Requests.Users;
using Domain.Constants.Enums;
using FluentValidation;

namespace API.Validations.Users;

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty().WithMessage("First Name is required");

        RuleFor(u => u.LastName)
            .NotEmpty().WithMessage("Last Name is required");

        RuleFor(u => u.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

        RuleFor(u => u.Email)
            .EmailAddress().WithMessage("Email Address is invalid")
            .NotEmpty().WithMessage("Email Address is required.");

        RuleFor(u => u.Role)
            .IsInEnum()
            .Must(role => role >= Role.Admin && role <= Role.Accountant)
            .WithMessage("Role must be valid long and is required.");
    }
}