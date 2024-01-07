using API.DTOs.Requests.Users;
using FluentValidation;

namespace API.Validations.Users
{
    public class AddCompanyDetailRequestValidator : AbstractValidator<AddCompanyDetailRequest>
    {
        public AddCompanyDetailRequestValidator()
        {
            RuleFor(x => x.IsCompanyAdmin)
                .NotNull();
            RuleFor(x => x.CompanyAddressId)
                .NotNull();
            RuleFor(x => x.CompanyId)
                .NotNull();
        }
    }
}
