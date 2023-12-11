using API.DTOs.Requests.TicketSolutions;
using FluentValidation;

namespace API.Validations.TicketSolutions
{
    public class CreateTicketSolutionValidator : AbstractValidator<CreateTicketSolutionRequest>
    {
        public CreateTicketSolutionValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title should not exceed 100 characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.");

            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage("Category ID is required.")
                .GreaterThan(0).WithMessage("Category ID should be greater than 0.");

            RuleFor(x => x.OwnerId)
                .GreaterThan(0).WithMessage("Owner ID should be greater than 0.");

            RuleFor(x => x.ExpiredDate)
                .Must((dto, expiredDate) => dto.ReviewDate == null || expiredDate == null || expiredDate > dto.ReviewDate)
                .WithMessage("Expired date should be null or later than the review date.");

            RuleFor(x => x.Keyword)
                .MaximumLength(255).WithMessage("Keyword should not exceed 255 characters.");

            RuleFor(x => x.InternalComments)
                .MaximumLength(500).WithMessage("Internal comments should not exceed 500 characters.");
        }
    }
}
