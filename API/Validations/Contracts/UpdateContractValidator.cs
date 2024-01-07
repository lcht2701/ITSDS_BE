using API.DTOs.Requests.Contracts;
using FluentValidation;

namespace API.Validations.Contracts
{
    public class UpdateContractValidator : AbstractValidator<UpdateContractRequest>
    {
        public UpdateContractValidator()
        {
            RuleFor(x => x.ContractNumber)
                .NotEmpty().WithMessage("ContractNumber is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description should not exceed 500 characters.");

            RuleFor(x => x.Value)
                .GreaterThan(0).When(x => x.Value.HasValue)
                .WithMessage("Value should be greater than 0.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration should be greater than 0.");

            RuleFor(x => x.CompanyId)
                .GreaterThan(0).WithMessage("Invalid Company");

            RuleFor(x => x.AttachmentUrls)
                .NotNull().WithMessage("AttachmentUrls cannot be null.");

            RuleFor(x => x.Note)
                .MaximumLength(1000).WithMessage("Note should not exceed 1000 characters.");
        }
    }
}
