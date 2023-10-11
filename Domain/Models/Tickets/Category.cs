using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets
{
    public class Category : BaseEntity
    {
        public Category()
        {
            Tickets = new HashSet<Ticket>();
            TicketSolutions = new HashSet<TicketSolution>();
        }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? AssignedTechnicalId { get; set; }

        public virtual User? AssignedTechnical { get; set; }

        [JsonIgnore]
        public virtual ICollection<Ticket>? Tickets { get; set; }
        [JsonIgnore]
        public virtual ICollection<TicketSolution>? TicketSolutions { get; set; }
    }
}
