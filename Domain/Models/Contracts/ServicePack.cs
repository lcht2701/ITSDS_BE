using System.Text.Json.Serialization;

namespace Domain.Models.Contracts
{
    public partial class ServicePack : BaseEntity
    {
        public ServicePack()
        {
            ServiceServicePacks = new HashSet<ServiceServicePack>();
        }

        public string? Description { get; set; }

        public string? Price { get; set; }

        [JsonIgnore]
        public virtual ICollection<ServiceServicePack>? ServiceServicePacks { get; set; }
    }
}
