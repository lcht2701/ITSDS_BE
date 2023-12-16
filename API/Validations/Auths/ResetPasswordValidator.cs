using API.DTOs.Requests.Auths;
using FluentValidation;

namespace API.Validations.Auths
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator()
        {
            RuleFor(request => request.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters long");

            RuleFor(request => request.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(request => request.NewPassword).WithMessage("Passwords do not match");
        }
    }
}
