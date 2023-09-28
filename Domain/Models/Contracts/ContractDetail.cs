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
            AdditionalServices = new HashSet<Service>();
        }
        public Guid ContractId { get; set; }

        public virtual Contract Contract { get; set; }

        public Guid? ServicePackId { get; set; }

        public virtual ServicePack ServicePack { get; set; }

        public virtual ICollection<Service>? AdditionalServices { get; set; }

    }
}
