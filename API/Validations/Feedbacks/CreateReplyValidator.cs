using API.DTOs.Requests.Feedbacks;
using FluentValidation;

namespace API.Validations.Feedbacks
{
    public class CreateReplyValidator : AbstractValidator<CreateReplyRequest>
    {
        public CreateReplyValidator()
        {
            RuleFor(x => x.Comment)
               .NotEmpty().WithMessage("Comment is required.")
               .MaximumLength(500).WithMessage("Comment should not exceed 500 characters.");

            RuleFor(x => x.ParentFeedbackId)
                .NotNull().WithMessage("ParentFeedbackId is required.")
                .GreaterThan(0).WithMessage("ParentFeedbackId should be greater than 0.");

            RuleFor(x => x.IsPublic)
                .NotNull().WithMessage("IsPublic is required.");
        }
    }
}
