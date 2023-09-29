using Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets;

public partial class Ticket : BaseEntity
{
    public Ticket()
    {
        TicketTasks = new HashSet<TicketTask>();
        TicketApprovals = new HashSet<TicketApproval>();
    }

    public string Title { get; set; }

    public string? Description { get; set; }

    public TicketStatus TicketStatus { get; set; }

    public string? Note { get; set; }

    public DateTime EndDate { get; set; }

    public int RequesterId { get; set; }

    public int AssignmentId { get; set; }

    public virtual Assignment Assignment { get; set; }

    public int ServiceId { get; set; }

    [JsonIgnore]
    public virtual ICollection<TicketTask> TicketTasks { get; set; }

    [JsonIgnore]
    public virtual ICollection<TicketApproval> TicketApprovals { get; set; }

    [JsonIgnore]
    public virtual ICollection<Feedback> Feedbacks { get; set; }


}
