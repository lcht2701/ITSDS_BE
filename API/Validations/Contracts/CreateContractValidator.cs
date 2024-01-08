using API.DTOs.Requests.Contracts;
using FluentValidation;

namespace API.Validations.Contracts
{
    public class CreateContractValidator : AbstractValidator<CreateContractRequest>
    {
        public CreateContractValidator()
        {
            RuleFor(x => x.ContractNumber)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description should not exceed 500 characters.");

            RuleFor(x => x.Value)
                .GreaterThan(0).When(x => x.Value.HasValue).WithMessage("Value should be greater than 0.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration should be greater than 0.");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company is required.")
                .GreaterThan(0).WithMessage("Invalid Company");

            RuleFor(x => x.ServiceIds)
                .NotNull().WithMessage("ServiceIds cannot be null.")
                .Must(x => x.Count > 0).WithMessage("At least one ServiceId is required.");

            RuleFor(x => x.Note)
                .MaximumLength(1000).WithMessage("Note should not exceed 1000 characters.");
        }
    }
}
