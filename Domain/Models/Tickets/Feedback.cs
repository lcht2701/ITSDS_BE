using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Tickets;

public partial class Feedback : BaseEntity
{
    public int Rating { get; set; }

    public string? Description { get; set; }

    public Guid TicketId { get; set; }

    public virtual Ticket Ticket { get; set; }

    public Guid CustomerId { get; set; }
}
