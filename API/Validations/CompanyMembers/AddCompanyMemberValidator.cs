using API.DTOs.Requests.CompanyMembers;
using FluentValidation;

namespace API.Validations.CompanyMembers
{
    public class AddCompanyMemberValidator : AbstractValidator<AddCompanyMemberRequest>
    {
        public AddCompanyMemberValidator()
        {
            RuleFor(x => x.MemberPosition)
                .MaximumLength(255).WithMessage("MemberPosition should not exceed 255 characters.");
        }
    }
}
