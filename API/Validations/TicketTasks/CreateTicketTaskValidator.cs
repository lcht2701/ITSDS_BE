using API.DTOs.Requests.TicketTasks;
using FluentValidation;

namespace API.Validations.TicketTasks
{
    public class CreateTicketTaskValidator : AbstractValidator<CreateTicketTaskRequest>
    {
        public CreateTicketTaskValidator()
        {
            RuleFor(x => x.TicketId)
                .NotNull().WithMessage("Ticket ID is required.")
                .GreaterThan(0).WithMessage("Ticket ID should be greater than 0.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title should not exceed 100 characters.");

            RuleFor(x => x.Priority)
                .IsInEnum()
                .WithMessage("Priority is required.");

            RuleFor(x => x.ScheduledStartTime)
                .GreaterThan(DateTime.Today).When(x => x.ScheduledStartTime != null)
                .WithMessage("Scheduled start time should be in the future.");

            RuleFor(x => x.ScheduledEndTime)
                .GreaterThan(x => x.ScheduledStartTime).When(x => x.ScheduledEndTime != null)
                .WithMessage("Scheduled end time should be greater than start time.");

            RuleFor(x => x.Progress)
                .InclusiveBetween(0, 100)
                .WithMessage("Progress should be between 0 and 100 if provided.");
        }
    }
}
