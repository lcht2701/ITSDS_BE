﻿using API.DTOs.Requests.Tickets;
using FluentValidation;

namespace API.Validations.Tickets
{
    public class CreateTicketCustomerValidator : AbstractValidator<CreateTicketCustomerRequest>
    {
        public CreateTicketCustomerValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title should not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description should not exceed 500 characters.");

            RuleFor(x => x.ServiceId)
                .NotNull().WithMessage("Service ID is required.")
                .GreaterThan(0).WithMessage("Category ID should be greater than 0.");
        }
    }
}