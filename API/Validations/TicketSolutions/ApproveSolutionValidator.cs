using API.DTOs.Requests.TicketSolutions;
using FluentValidation;

namespace API.Validations.TicketSolutions
{
    public class ApproveSolutionValidator : AbstractValidator<ApproveSolutionRequest>
    {
        public ApproveSolutionValidator()
        {
            RuleFor(x => x.Duration)
                .InclusiveBetween(1,24).WithMessage("Duration of a Solution is between 1-24 months");
        }
    }
}
