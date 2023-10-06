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
        }

        public int? Name { get; set; }

        public int? Description { get; set; }

        public int? AssignedTechnicalId { get; set; }

        [JsonIgnore]
        public virtual User? AssignedTechnical { get; set; }
        [JsonIgnore]
        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
}
