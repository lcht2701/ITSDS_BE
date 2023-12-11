using API.DTOs.Requests.Feedbacks;
using FluentValidation;

namespace API.Validations.Feedbacks
{
    public class UpdateFeedbackValidator : AbstractValidator<UpdateFeedbackRequest>
    {
        public UpdateFeedbackValidator()
        {
            RuleFor(x => x.Comment)
               .NotEmpty().WithMessage("Comment is required.")
               .MaximumLength(500).WithMessage("Comment should not exceed 500 characters.");

            RuleFor(x => x.IsPublic)
                .NotNull().WithMessage("IsPublic is required.");
        }
    }
}
