using API.DTOs.Requests.Users;
using FluentValidation;

namespace API.Validations.Users
{
    public class CreateUserOrCustomerAdminValidator : AbstractValidator<CreateUserOrCustomerAdmin>
    {
        public CreateUserOrCustomerAdminValidator()
        {
            RuleFor(x => x.IsCompanyAdmin)
                .NotNull();
        }
    }
}
