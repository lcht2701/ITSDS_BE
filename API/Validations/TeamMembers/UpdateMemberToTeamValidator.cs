using API.DTOs.Requests.TeamMembers;
using FluentValidation;

namespace API.Validations.TeamMembers
{
    public class UpdateMemberToTeamValidator : AbstractValidator<UpdateTeamMemberRequest>
    {
        public UpdateMemberToTeamValidator()
        {
            RuleFor(x => x.Expertises).NotNull().MaximumLength(100);
        }
    }
}
