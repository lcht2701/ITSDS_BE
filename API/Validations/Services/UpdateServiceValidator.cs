using API.DTOs.Requests.Services;
using FluentValidation;

namespace API.Validations.Services
{
    public class UpdateServiceValidator : AbstractValidator<UpdateServiceRequest>
    {
        public UpdateServiceValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(100);
        }
    }
}
