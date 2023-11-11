using API.DTOs.Requests.Tickets;
using FluentValidation;

namespace API.Validations.Tickets
{
    public class TechnicianAddDetailValidator : AbstractValidator<TechnicianAddDetailRequest>
    {
        public TechnicianAddDetailValidator()
        {
            RuleFor(u => u.Impact)
                .IsInEnum();
            RuleFor(u => u.ImpactDetail)
                .MaximumLength(100).WithMessage("Impact detail should not exceed 100 characters.");
            RuleFor(u => u.Urgency)
                .IsInEnum();
        }
    }
}