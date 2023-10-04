using Domain.Constants;
using Domain.Models.Contracts;
using System.Text.Json.Serialization;

namespace Domain.Models.Tickets;

public partial class Ticket : BaseEntity
{
    public Ticket()
    {
        Assignments = new HashSet<Assignment>();
        TicketTasks = new HashSet<TicketTask>();
        TicketApprovals = new HashSet<TicketApproval>();
    }
    public int? RequesterId { get; set; }

    public int? CompanyId { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public int? ServicePackId { get; set; }

    public int? ServiceId { get; set; }

    public TicketStatus? TicketStatus { get; set; }

    public Priority? Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? ClosedDate { get; set; }

    public int? TeamId { get; set; }

    public string? AttachmentUrl { get; set; }  

    [JsonIgnore]
    public virtual User Requester { get; set; }    
    [JsonIgnore]
    public virtual ServicePack ServicePack { get; set; }
    [JsonIgnore]
    public virtual Service Service { get; set; }
    [JsonIgnore]
    public virtual Team Team { get; set; }
    [JsonIgnore]
    public virtual Company Company { get; set; }

    [JsonIgnore]
    public virtual ICollection<Assignment> Assignments { get; set; }

    [JsonIgnore]
    public virtual ICollection<TicketTask> TicketTasks { get; set; }

    [JsonIgnore]
    public virtual ICollection<TicketApproval> TicketApprovals { get; set; }
}
