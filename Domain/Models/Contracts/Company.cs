using System.Text.Json.Serialization;

namespace Domain.Models.Contracts
{
    public partial class Company : BaseEntity
    {
        public Company()
        {
            Contracts = new HashSet<Contract>();
            CompanyAddresses = new HashSet<CompanyAddress>();
        }

        public string CompanyName { get; set; }

        public string? TaxCode { get; set; }

        public string? Website { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string? LogoUrl { get; set; }

        public string? FieldOfBusiness { get; set; }

        public bool IsActive { get; set; }

        [JsonIgnore]
        public virtual ICollection<Contract>? Contracts { get; set; }
        [JsonIgnore]
        public virtual ICollection<CompanyAddress>? CompanyAddresses { get; set; }

    }
}
