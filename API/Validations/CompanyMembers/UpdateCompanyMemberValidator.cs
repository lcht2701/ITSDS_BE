using API.DTOs.Requests.CompanyMembers;
using FluentValidation;

namespace API.Validations.CompanyMembers
{
    public class UpdateCompanyMemberValidator : AbstractValidator<UpdateCompanyMemberRequest>
    {
        public UpdateCompanyMemberValidator()
        {
            RuleFor(x => x.IsCompanyAdmin)
                .NotNull().WithMessage("Field must not be null");

            RuleFor(x => x.MemberPosition)
                .MaximumLength(255).WithMessage("MemberPosition should not exceed 255 characters.");
        }
    }
}
