﻿using API.DTOs.Requests.Services;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace API.Validations.Services
{
    public class CreateServiceValidator : AbstractValidator<CreateServiceRequest>
    {
        public CreateServiceValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.");
            RuleFor(x => x.Description)
                .MaximumLength(100);
            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Amount should be greater than or equal to 0.");
        }
    }
}