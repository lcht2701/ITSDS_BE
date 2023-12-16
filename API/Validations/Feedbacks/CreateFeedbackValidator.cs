using API.DTOs.Requests.Feedbacks;
using FluentValidation;

namespace API.Validations.Feedbacks
{
    public class CreateFeedbackValidator : AbstractValidator<CreateFeedbackRequest>
    {
        public CreateFeedbackValidator()
        {
            RuleFor(x => x.SolutionId)
                .NotNull().WithMessage("SolutionId is required.")
                .GreaterThan(0).WithMessage("SolutionId should be greater than 0.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required.")
                .MaximumLength(500).WithMessage("Comment should not exceed 500 characters.");

            RuleFor(x => x.IsPublic)
                .NotNull().WithMessage("IsPublic is required.");
        }
    }
}
