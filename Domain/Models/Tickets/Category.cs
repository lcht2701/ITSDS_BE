using Domain.Models.Contracts;
using System.Text.Json.Serialization;

namespace Domain.Models.Tickets
{
    public class Category : BaseEntity
    {
        public Category()
        {
            Tickets = new HashSet<Ticket>();
            TicketSolutions = new HashSet<TicketSolution>();
            Services = new HashSet<Service>();
            Teams = new HashSet<Team>();
        }

        public string Name { get; set; }

        public string? Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Team>? Teams { get; set; }
        [JsonIgnore]
        public virtual ICollection<Service>? Services { get; set; }
        [JsonIgnore]
        public virtual ICollection<Ticket>? Tickets { get; set; }
        [JsonIgnore]
        public virtual ICollection<TicketSolution>? TicketSolutions { get; set; }
    }
}
