using API.DTOs.Requests.Payments;
using FluentValidation;

namespace API.Validations.Payments
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.ContractId)
                .NotEmpty().WithMessage("ContractId is required.")
                .GreaterThan(0).WithMessage("ContractId should be greater than 0.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Description should not exceed 255 characters.");

            RuleFor(x => x.StartDateOfPayment)
                .NotEmpty().WithMessage("Start Date Of Payment is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start Date cannot be in the past");

            RuleFor(x => x.DaysAmountForPayment)
                .NotEmpty().WithMessage("Duration is required.")
                .GreaterThan(0).WithMessage("Duration should be greater than 0.");

            RuleFor(x => x.Note)
                .MaximumLength(255).WithMessage("Note should not exceed 255 characters.");
        }
    }
}
