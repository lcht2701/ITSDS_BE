using API.DTOs.Requests.TicketSolutions;
using FluentValidation;

namespace API.Validations.TicketSolutions
{
    public class UpdateTicketSolutionValidator : AbstractValidator<UpdateTicketSolutionRequest>
    {
        public UpdateTicketSolutionValidator()
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

            RuleFor(x => x.Keyword)
                .MaximumLength(255).WithMessage("Keyword should not exceed 255 characters.");
        }
    }
}
