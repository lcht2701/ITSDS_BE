using System.Text.Json.Serialization;

namespace Domain.Models.Contracts
{
    public partial class Department : BaseEntity
    {
        public Department()
        {
            CompanyMembers = new HashSet<CompanyMember>();
        }

        public int CompanyId { get; set; }

        public string Address { get; set; }

        public string? PhoneNumber { get; set; }

        public virtual Company? Company { get; set; }

        [JsonIgnore]
        public virtual ICollection<CompanyMember>? CompanyMembers { get; set; }
    }
}
