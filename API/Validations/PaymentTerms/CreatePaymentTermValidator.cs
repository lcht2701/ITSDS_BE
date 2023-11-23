using API.DTOs.Requests.PaymentTerms;
using FluentValidation;

namespace API.Validations.PaymentTerms
{
    public class CreatePaymentTermValidator : AbstractValidator<CreatePaymentTermRequest>
    {
        public CreatePaymentTermValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEmpty().WithMessage("PaymentId is required.")
                .GreaterThan(0).WithMessage("PaymentId should be greater than 0.");

            RuleFor(x => x.NumberOfPayments)
                .NotEmpty().WithMessage("NumberOfPayments is required.")
                .GreaterThan(0).WithMessage("NumberOfPayments should be greater than 0.");

            RuleFor(x => x.InitialPaymentAmount)
                .NotEmpty().WithMessage("InitialPaymentAmount is required.")
                .GreaterThan(0).WithMessage("InitialPaymentAmount should be greater than 0.");

            RuleFor(x => x.FirstDateOfPayment)
                .NotEmpty().WithMessage("FirstDateOfPayment is required.")
                .Must(date => date >= DateTime.UtcNow).WithMessage("FirstDateOfPayment should be a future date.");

            RuleFor(x => x.Duration)
                .NotEmpty().WithMessage("Duration is required.")
                .GreaterThan(0).WithMessage("Duration should be greater than 0.");
        }
    }
}
