using API.DTOs.Requests.Companies;
using FluentValidation;

namespace API.Validations.Companies;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.");

        RuleFor(x => x.TaxCode)
                .NotEmpty().WithMessage("Tax code is required.")
                .Length(10, 13).WithMessage("Tax code should be between 10 and 13 characters.");

        RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).When(x => x.PhoneNumber != null)
                .WithMessage("Phone number should not exceed 15 characters.")
                .Matches(@"^\+?[0-9-]*$").WithMessage("Invalid phone number format.");

        RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address format.");

        RuleFor(x => x.Website)
                .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.Website))
                .WithMessage("Invalid URL format for Website.");

        RuleFor(x => x.LogoUrl)
                .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.LogoUrl))
                .WithMessage("Invalid URL format for LogoUrl.");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}