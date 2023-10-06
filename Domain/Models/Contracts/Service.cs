using Domain.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Contracts
{
    public partial class Service : BaseEntity
    {
        public Service()
        {
            Tickets = new HashSet<Ticket>();
            ContractDetails = new HashSet<ContractDetail>();
        }
        public string? Description { get; set; }

        public string? Type { get; set; }

        public string? Amount { get; set; }

        public int? ServicePackId { get; set; }

        [JsonIgnore]
        public virtual ServicePack? ServicePacks { get; set; }
        [JsonIgnore]
        public virtual ICollection<Ticket>? Tickets { get; set; }
        [JsonIgnore]
        public virtual ICollection<ContractDetail>? ContractDetails { get; set; }

    }
}
