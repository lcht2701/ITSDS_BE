using API.DTOs.Requests.Tickets;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;

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

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Title is required.")
                .NotNull().WithMessage("Priority is required.");

            RuleFor(x => x.ServiceId)
                .NotNull().WithMessage("Service ID is required.")
                .GreaterThan(0).WithMessage("Category ID should be greater than 0.");

            RuleFor(x => x.Type)
                .NotNull().WithMessage("Type is required.");

            RuleFor(x => x.Street)
                .NotNull().WithMessage("Street is required")
                .MaximumLength(200).WithMessage("Street should not exceed 200 characters.");

            RuleFor(x => x.Ward)
                .NotNull().WithMessage("Ward is required");

            RuleFor(x => x.District)
                .NotNull().WithMessage("District is required");

            RuleFor(x => x.City)
                .NotNull().WithMessage("City is required");

        }
    }
}