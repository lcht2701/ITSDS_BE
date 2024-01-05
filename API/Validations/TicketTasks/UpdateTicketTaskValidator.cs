using API.DTOs.Requests.TicketTasks;
using FluentValidation;

namespace API.Validations.TicketTasks
{
    public class UpdateTicketTaskValidator : AbstractValidator<UpdateTicketTaskRequest>
    {
        public UpdateTicketTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title should not exceed 100 characters.");

            RuleFor(x => x.TaskStatus)
                .IsInEnum().When(x => x.TaskStatus != null)
                .WithMessage("Invalid TaskStatus value.");

            RuleFor(x => x.Priority)
                .IsInEnum().When(x => x.Priority != null)
                .WithMessage("Priority is required.");

            RuleFor(x => x.ScheduledEndTime)
                .GreaterThan(x => x.ScheduledStartTime).When(x => x.ScheduledEndTime != null)
                .WithMessage("Scheduled end time should be greater than start time.");

            RuleFor(x => x.Progress)
                .InclusiveBetween(0, 100)
                .WithMessage("Progress should be between 0 and 100 if provided.");

            RuleFor(x => x.Note)
                .MaximumLength(200).WithMessage("Note should not exceed 200 characters.");
        }
    }
}
