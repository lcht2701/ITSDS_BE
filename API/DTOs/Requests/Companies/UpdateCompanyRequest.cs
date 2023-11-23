using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.Companies;

public class UpdateCompanyRequest : IMapTo<Company>
{
    public string? CompanyName { get; set; }

    public string? TaxCode { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? CompanyAddress { get; set; }

    public string? LogoUrl { get; set; }

    public string? FieldOfBusiness { get; set; }

    public bool? IsActive { get; set; }

    public int? CustomerAdminId { get; set; }
}
