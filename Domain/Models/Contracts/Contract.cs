using Domain.Constants.Enums;
using System.Text.Json.Serialization;

namespace Domain.Models.Contracts
{
    public partial class Contract : BaseEntity
    {
        public Contract()
        {
            ServiceContracts = new HashSet<ServiceContract>();
            Payments = new HashSet<Payment>();
        }

        public string ContractNumber { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public double Value { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int CompanyId { get; set; }

        public ContractStatus Status { get; set; }

        public virtual Company? Company { get; set; }

        [JsonIgnore]
        public virtual ICollection<ServiceContract>? ServiceContracts { get; set; }

        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
