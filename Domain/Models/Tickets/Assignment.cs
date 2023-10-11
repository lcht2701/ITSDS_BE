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

    public int? TeamId { get; set; }

    public virtual User? Technician { get; set; }
    public virtual Team? Team { get; set; }
    public virtual Ticket? Ticket { get; set; }
}
