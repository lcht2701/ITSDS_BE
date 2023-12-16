using API.DTOs.Requests.Auths;
using FluentValidation;

namespace API.Validations.Auths
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("CurrentPassword is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("NewPassword is required.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("ConfirmNewPassword is required.")
                .Equal(x => x.NewPassword).WithMessage("ConfirmNewPassword should match NewPassword.");
        }
    }
}
