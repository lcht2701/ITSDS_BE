using API.DTOs.Requests.Modes;
using FluentValidation;

namespace API.Validations.Modes
{
    public class CreateModeValidator : AbstractValidator<CreateModeRequest>
    {
        public CreateModeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name should not exceed 255 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description should not exceed 500 characters.");
        }
    }
}
