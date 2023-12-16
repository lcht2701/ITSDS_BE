using API.DTOs.Requests.CompanyMembers;
using FluentValidation;

namespace API.Validations.CompanyMembers
{
    public class AddCompanyMemberValidator : AbstractValidator<AddCompanyMemberRequest>
    {
        public AddCompanyMemberValidator()
        {
            RuleFor(x => x.MemberId)
              .NotNull().WithMessage("MemberId is required.")
              .GreaterThan(0).WithMessage("MemberId should be greater than 0.");

            RuleFor(x => x.CompanyId)
                .NotNull().WithMessage("CompanyId is required.")
                .GreaterThan(0).WithMessage("CompanyId should be greater than 0.");

            RuleFor(x => x.MemberPosition)
                .MaximumLength(255).WithMessage("MemberPosition should not exceed 255 characters.");
        }
    }
}
