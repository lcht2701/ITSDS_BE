using API.DTOs.Requests.PaymentTerms;
using FluentValidation;

namespace API.Validations.PaymentTerms
{
    public class UpdatePaymentTermValidator : AbstractValidator<UpdatePaymentTermRequest>
    {
        public UpdatePaymentTermValidator()
        {
            RuleFor(x => x.IsPaid)
                .Must(BeAValidBoolean).WithMessage("IsPaid must be a valid boolean value.");

            RuleFor(x => x.Note)
                .MaximumLength(255).WithMessage("Note should not exceed 255 characters.");
        }

        private bool BeAValidBoolean(bool? value)
        {
            // Validation for boolean values (true, false, null)
            return value.HasValue || !value.HasValue;
        }
    }
}
