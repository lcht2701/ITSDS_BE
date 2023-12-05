using API.DTOs.Requests.Tickets;
using FluentValidation;

namespace API.Validations.Tickets
{
    public class UpdateTicketCustomerValidator : AbstractValidator<UpdateTicketCustomerRequest>
    {
        public UpdateTicketCustomerValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title should not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description should not exceed 500 characters.");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Title is required.")
                .NotNull().WithMessage("Priority is required.");

            RuleFor(x => x.Street)
                .NotNull().WithMessage("Street is required")
                .MaximumLength(200).WithMessage("Street should not exceed 200 characters.");

            RuleFor(x => x.Ward)
                .NotEmpty().WithMessage("Ward is required");

            RuleFor(x => x.District)
                .NotEmpty().WithMessage("District is required");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required");
        }
    }
}