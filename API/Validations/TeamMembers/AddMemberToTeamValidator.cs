using API.DTOs.Requests.TeamMembers;
using FluentValidation;

namespace API.Validations.TeamMembers
{
    public class AddMemberToTeamValidator : AbstractValidator<AddMemberToTeamRequest>
    {
        public AddMemberToTeamValidator()
        {
            RuleFor(x => x.MemberId).NotNull();
            RuleFor(x => x.TeamId).NotNull();
            RuleFor(x => x.Expertises).NotNull().MaximumLength(100);
        }
    }
}
