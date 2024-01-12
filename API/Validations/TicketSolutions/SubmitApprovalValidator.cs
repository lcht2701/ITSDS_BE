using API.DTOs.Requests.TicketSolutions;
using FluentValidation;

namespace API.Validations.TicketSolutions
{
    public class SubmitApprovalValidator : AbstractValidator<SubmitApprovalRequest>
    {
        public SubmitApprovalValidator()
        {
            RuleFor(x => x.ManagerId)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
