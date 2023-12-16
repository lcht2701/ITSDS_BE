using API.DTOs.Requests.Teams;
using FluentValidation;

namespace API.Validations.Teams
{
    public class UpdateTeamValidator : AbstractValidator<UpdateTeamRequest>
    {
        public UpdateTeamValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.Description)
                .MaximumLength(250);
            RuleFor(x => x.Location)
                .MaximumLength(250);
            RuleFor(x => x.IsActive)
                .NotNull();
        }
    }
}
