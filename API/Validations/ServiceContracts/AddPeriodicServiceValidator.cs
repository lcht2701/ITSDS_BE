using API.DTOs.Requests.ServiceContracts;
using FluentValidation;

namespace API.Validations.ServiceContracts;

public class AddPeriodicServiceValidator : AbstractValidator<AddPeriodicService>
{
    public AddPeriodicServiceValidator()
    {
        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("Frequency Days is required")
            .GreaterThan(0).WithMessage("Frequency Days must be greater than 0");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start Date is required");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End Date is required")
            .GreaterThan(x => x.StartDate).WithMessage("End Date must be greater than Start Date");

    }
}