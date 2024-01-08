using API.DTOs.Requests.Payments;
using FluentValidation;

namespace API.Validations.Payments
{
    public class UpdatePaymentValidator : AbstractValidator<UpdatePaymentRequest>
    {
        public UpdatePaymentValidator()
        {
            RuleFor(x => x.Note)
                .MaximumLength(255).WithMessage("Note should not exceed 255 characters.");
        }
    }
}
