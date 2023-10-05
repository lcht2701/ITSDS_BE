using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Contracts
{
    public partial class ContractDetail : BaseEntity
    {
        public ContractDetail()
        {
        }
        public int? ContractId { get; set; }

        public int? ServicePackId { get; set; }

        public int? AdditionalServiceId { get; set; }

        [JsonIgnore]
        public virtual Contract? Contract { get; set; }
        [JsonIgnore]
        public virtual ServicePack? ServicePack { get; set; }
        [JsonIgnore]
        public virtual Service? AdditionalService { get; set; }

    }
}
