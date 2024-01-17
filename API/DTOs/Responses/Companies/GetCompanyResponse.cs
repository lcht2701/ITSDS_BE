using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Responses.Companies
{
    public class GetCompanyResponse : IMapFrom<Company>
    {
        public int Id { get; init; }

        public string? CompanyName { get; set; }

        public string? TaxCode { get; set; }

        public string? Website { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? LogoUrl { get; set; }

        public string? FieldOfBusiness { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
