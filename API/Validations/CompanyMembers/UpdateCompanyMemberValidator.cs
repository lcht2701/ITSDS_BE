﻿using API.DTOs.Requests.CompanyMembers;
using FluentValidation;

namespace API.Validations.CompanyMembers
{
    public class UpdateCompanyMemberValidator : AbstractValidator<UpdateCompanyMemberRequest>
    {
        public UpdateCompanyMemberValidator()
        {
            RuleFor(x => x.IsCompanyAdmin)
                .NotEmpty().WithMessage("CompanyId is required.");

            RuleFor(x => x.MemberPosition)
                .MaximumLength(255).WithMessage("MemberPosition should not exceed 255 characters.");
        }
    }
}
