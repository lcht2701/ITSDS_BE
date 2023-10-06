using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets;

public partial class Assignment : BaseEntity
{
    public int? TicketId { get; set; }

    public int? TechnicianId { get; set; }

    //public int? TeamId { get; set; }

    [JsonIgnore]
    public virtual User? Technician { get; set; }
    //[JsonIgnore]
    //public virtual Team? Team { get; set; }
    [JsonIgnore]
    public virtual Ticket Ticket { get; set; }
}
