using API.DTOs.Requests.Contracts;
using FluentValidation;

namespace API.Validations.Contracts
{
    public class UpdateContractValidator : AbstractValidator<UpdateContractRequest>
    {
        public UpdateContractValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description should not exceed 500 characters.");

            RuleFor(x => x.Value)
                .GreaterThan(0).When(x => x.Value.HasValue).WithMessage("Value should be greater than 0.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");

            RuleFor(x => x.EndDate)
                .NotNull().WithMessage("EndDate is required.")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("EndDate should be greater than or equal to StartDate.");

            RuleFor(x => x.ParentContractId)
                .GreaterThanOrEqualTo(0).WithMessage("ParentContractId should be greater than or equal to 0.");

            RuleFor(x => x.AccountantId)
                .NotNull().WithMessage("An employee is required to manage the contract.")
                .GreaterThan(0).WithMessage("AccountantId should be greater than 0.");

            RuleFor(x => x.CompanyId)
                 .NotEmpty().WithMessage("Company is required.")
                 .GreaterThan(0).WithMessage("CompanyId should be greater than 0.");
        }
    }
}
