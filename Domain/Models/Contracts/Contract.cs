using Domain.Constants.Enums;
using System.Text.Json.Serialization;

namespace Domain.Models.Contracts
{
    public partial class Contract : BaseEntity
    {
        public Contract()
        {
            Renewals = new HashSet<Renewal>();
            ServiceContracts = new HashSet<ServiceContract>();
        }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? Value { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsRenewed { get; set; }

        public int? ParentContractId { get; set; }

        public int? AccountantId { get; set; }

        public string? AttachmentURl { get; set; }

        public int? CompanyId { get; set; }

        public ContractStatus? Status { get; set; }

        public virtual User? Accountant { get; set; }

        public virtual Company? Company { get; set; }

        [JsonIgnore]
        public virtual ICollection<ServiceContract>? ServiceContracts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Renewal> Renewals { get; set; }
    }
}
