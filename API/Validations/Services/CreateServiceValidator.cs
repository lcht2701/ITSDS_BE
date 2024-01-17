using API.DTOs.Requests.Services;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace API.Validations.Services
{
    public class CreateServiceValidator : AbstractValidator<CreateServiceRequest>
    {
        public CreateServiceValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(100);
        }
    }
}
