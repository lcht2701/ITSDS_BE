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
            Services = new HashSet<Service>();
        }

        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Service> Services { get; set; }
    }
}
