using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Contracts
{
    public partial class ServicePack : BaseEntity
    {
        public ServicePack()
        {
            ContractDetails = new HashSet<ContractDetail>();
        }
        public string? Description { get; set; }
        public string? Price { get; set; }

        [JsonIgnore]
        public virtual ICollection<Service>? Services { get; set; }
        [JsonIgnore]
        public virtual ICollection<ContractDetail>? ContractDetails { get; set; }
    }
}
