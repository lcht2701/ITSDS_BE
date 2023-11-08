using API.Mappings;
using Domain.Models.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Companies;

public class CreateCompanyRequest : IMapTo<Company>
{
    [Required]
    public string? CompanyName { get; set; }

    [Required]
    [MinLength(10), MaxLength(13)]
    public string? TaxCode { get; set; }

    [Required]
    [Phone]
    public string? PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? CompanyAddress { get; set; }

    public string? LogoUrl { get; set; }

    public string? FieldOfBusiness { get; set; }

    [DefaultValue(true)]
    public bool? IsActive { get; set; }

    public int? CustomerAdminId { get; set; }
}
