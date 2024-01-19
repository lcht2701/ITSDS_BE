using API.Mappings;
using Domain.Models.Contracts;
using System.ComponentModel;

namespace API.DTOs.Requests.Companies;

public class CreateCompanyRequest : IMapTo<Company>
{
    public string? CompanyName { get; set; }

    public string? TaxCode { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? LogoUrl { get; set; }

    public string? FieldOfBusiness { get; set; }

    [DefaultValue(true)]
    public bool? IsActive { get; set; }

    public string CompanyAddress { get; set; }
}
