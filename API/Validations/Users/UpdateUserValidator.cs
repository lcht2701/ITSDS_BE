﻿using API.DTOs.Requests.Users;
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
                .Must((user, email) =>
                {
                    // Check uniqueness only if the email is changed
                    if (user.Email != email)
                    {
                        // Check if the email is already in use
                        return _userRepository.FirstOrDefaultAsync(x => x.Email.Equals(email)).Result == null;
                    }
                    return true; // If email is not changed, validation passes
                }).WithMessage("Email Address is already in use.")
                .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(u => u.Role)
                .IsInEnum()
                .Must(role => role >= Role.Admin && role <= Role.Accountant)
                .WithMessage("Role must be valid and is required.");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive is required.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value.")
                .Must(gender => gender >= Gender.Male && gender <= Gender.Female)
                .WithMessage("Role must be valid and is required.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15)
                .WithMessage("Phone number should not exceed 15 characters.")
                .Matches(@"^\+?[0-9-]*$").WithMessage("Invalid phone number format.");
        }
    }
}


