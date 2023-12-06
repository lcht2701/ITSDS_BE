using API.DTOs.Requests.Payments;
using FluentValidation;

namespace API.Validations.Payments
{
    public class UpdatePaymentValidator : AbstractValidator<UpdatePaymentRequest>
    {
        public UpdatePaymentValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Description should not exceed 255 characters.");

            RuleFor(x => x.NumberOfTerms)
                .NotEmpty().WithMessage("Number Of Terms is required.")
                .GreaterThan(0).WithMessage("NumberOfTerms should be greater than 0.");

            RuleFor(x => x.Duration)
                .NotEmpty().WithMessage("Duration is required.")
                .GreaterThan(0).WithMessage("Duration should be greater than 0.");

            RuleFor(x => x.InitialPaymentAmount)
                .GreaterThanOrEqualTo(0).WithMessage("InitialPaymentAmount should be greater than or equal to 0.");

            RuleFor(x => x.Note)
                .MaximumLength(255).WithMessage("Note should not exceed 255 characters.");
        }
    }
}
