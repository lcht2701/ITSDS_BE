using System.Text.Json.Serialization;

namespace Domain.Models.Tickets;

public partial class Assignment : BaseEntity
{
    public int? TechnicianId { get; set; }

    public int? TeamId { get; set; }

    public int TicketId { get; set; }

    public virtual User? Technician { get; set; }

    public virtual Team? Team { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
