using Domain.Models.Tickets;
using System.Text.Json.Serialization;

namespace Domain.Models.Contracts
{
    public partial class Service : BaseEntity
    {
        public Service()
        {
            Tickets = new HashSet<Ticket>();
            ContractServices = new HashSet<ContractService>();
            ServiceServicePacks = new HashSet<ServiceServicePack>();
        }

        public string? Description { get; set; }

        public string? Type { get; set; }

        public string? Amount { get; set; }

        [JsonIgnore]
        public virtual ICollection<Ticket>? Tickets { get; set; }
        [JsonIgnore]
        public virtual ICollection<ServiceServicePack>? ServiceServicePacks { get; set; }
        [JsonIgnore]
        public virtual ICollection<ContractService>? ContractServices { get; set; }

    }
}
