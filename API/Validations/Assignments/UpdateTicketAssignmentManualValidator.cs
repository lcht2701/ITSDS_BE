using API.DTOs.Requests.Assignments;
using FluentValidation;

namespace API.Validations.Assignments
{
    public class UpdateTicketAssignmentManualValidator : AbstractValidator<UpdateTicketAssignmentManualRequest>
    {
        public UpdateTicketAssignmentManualValidator()
        {
            RuleFor(x => x.TechnicianId)
                .GreaterThanOrEqualTo(0).WithMessage("TechnicianId should be greater than or equal to 0.");

            RuleFor(x => x.TeamId)
                .GreaterThanOrEqualTo(0).WithMessage("TeamId should be greater than or equal to 0.");
        }
    }
}
