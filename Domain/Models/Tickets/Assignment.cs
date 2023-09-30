using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Tickets;

public partial class Assignment : BaseEntity
{
    public int TicketId { get; set; }

    public virtual Ticket Ticket { get; set; }

    public int TechnicianId { get; set; }

    public virtual User User { get; set; }
}
