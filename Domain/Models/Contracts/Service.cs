using Domain.Models.Tickets;
using System.Text.Json.Serialization;

namespace Domain.Models.Contracts
{
    public partial class Service : BaseEntity
    {
        public Service()
        {
            Tickets = new HashSet<Ticket>();
            ServiceContracts = new HashSet<ServiceContract>();
        }

        public string? Description { get; set; }

        public string? Type { get; set; }

        public double? Amount { get; set; }

        [JsonIgnore]
        public virtual ICollection<Ticket>? Tickets { get; set; }
        [JsonIgnore]
        public virtual ICollection<ServiceContract>? ServiceContracts { get; set; }

    }
}
