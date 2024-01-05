using API.DTOs.Requests.Users;
using FluentValidation;

namespace API.Validations.Users
{
    public class CreateUserOrCompanyAdminValidator : AbstractValidator<CreateUserOrCompanyAdmin>
    {
        public CreateUserOrCompanyAdminValidator()
        {
            RuleFor(x => x.IsCompanyAdmin)
                .NotNull();
        }
    }
}
