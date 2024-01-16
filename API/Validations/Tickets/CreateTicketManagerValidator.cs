﻿using API.DTOs.Requests.Tickets;
using FluentValidation;

namespace API.Validations.Tickets
{
    public class CreateTicketManagerValidator : AbstractValidator<CreateTicketManagerRequest>
    {
        public CreateTicketManagerValidator()
        {
            RuleFor(x => x.RequesterId)
                .NotNull().WithMessage("Requester ID is required.")
                .GreaterThan(0).WithMessage("Requester ID should be greater than 0.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title should not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description should not exceed 500 characters.");

            RuleFor(x => x.ModeId)
                .GreaterThan(0).WithMessage("Mode ID should be greater than 0.");

            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("Service ID should be greater than 0.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID should be greater than 0.");
            
            RuleFor(x => x.Priority)
                .IsInEnum();

            RuleFor(x => x.Impact)
                .IsInEnum();

            RuleFor(x => x.ImpactDetail)
                .MaximumLength(100).WithMessage("Impact detail should not exceed 100 characters.");

            RuleFor(x => x.ScheduledStartTime)
                .GreaterThanOrEqualTo(DateTime.Now)
                .WithMessage("Scheduled Start Time cannot be in the past");

            RuleFor(x => x.ScheduledEndTime)
                .GreaterThanOrEqualTo(x => x.ScheduledStartTime)
                .WithMessage("Scheduled End Time should be greater than or equal to Scheduled Start Time");
        }
    }
}
