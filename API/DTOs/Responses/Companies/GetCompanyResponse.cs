using API.Mappings;
using Domain.Models;
using Domain.Models.Contracts;
using System.Text.Json.Serialization;

namespace API.DTOs.Responses.Companies
{
    public class GetCompanyResponse : IMapFrom<Company>
    {
        public string? CompanyName { get; set; }

        public string? TaxCode { get; set; }

        public string? Website { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? CompanyAddress { get; set; }

        public string? LogoUrl { get; set; }

        public string? FieldOfBusiness { get; set; }

        public bool? isActive { get; set; }

        public int? CustomerAdminId { get; set; }

        public string? CustomerAdminName => $"{CustomerAdmin?.FirstName} {CustomerAdmin?.LastName}";

        [JsonIgnore]
        public virtual User? CustomerAdmin { get; set; }
    }
}
