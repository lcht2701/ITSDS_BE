using API.DTOs.Requests.Teams;
using FluentValidation;

namespace API.Validations.Teams
{
    public class CreateTeamValidator : AbstractValidator<CreateTeamRequest>
    {
        public CreateTeamValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.Description)
                .MaximumLength(250);
            RuleFor(x => x.Location)
                .MaximumLength(250);
        }
    }
}
