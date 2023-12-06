using API.DTOs.Requests.Tickets;
using FluentValidation;

namespace API.Validations.Tickets
{
    public class CreateTicketManagerValidator : AbstractValidator<CreateTicketManagerRequest>
    {
        public CreateTicketManagerValidator()
        {
            RuleFor(x => x.RequesterId)
                .NotNull().WithMessage("Requester ID is required.")
                .GreaterThan(0).WithMessage("Requester ID should be greater than 0.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title should not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description should not exceed 500 characters.");

            RuleFor(x => x.ModeId)
                .GreaterThan(0).WithMessage("Mode ID should be greater than 0.");

            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("Service ID should be greater than 0.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID should be greater than 0.");

            RuleFor(x => x.ImpactDetail)
                .MaximumLength(100).WithMessage("Impact detail should not exceed 100 characters.");

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
