using Domain.Constants;
using Domain.Models.Contracts;
using System.Text.Json.Serialization;

namespace Domain.Models.Tickets;

public partial class Ticket : BaseEntity
{
    public Ticket()
    {
        Assignments = new HashSet<Assignment>();
        TicketAnalysts = new HashSet<TicketAnalyst>();
        TicketTasks = new HashSet<TicketTask>();
    }
    public int? RequesterId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? ModeId { get; set; }

    public int? ServiceId { get; set; }

    public int? TeamId { get; set; }

    public TicketStatus? TicketStatus { get; set; }

    public Priority? Priority { get; set; }

    public DateTime? ScheduledStartTime { get; set; }

    public DateTime? ScheduledEndTime { get; set; }

    public DateTime? DueTime { get; set; }

    public DateTime? CompletedTime { get; set; }

    public Impact? Impact { get; set; }
    
    public Urgency? Urgency { get; set; }

    public int? CategoryId { get; set; }

    public string? AttachmentUrl { get; set; }

    [JsonIgnore]
    public virtual User? Requester { get; set; }  
    [JsonIgnore]
    public virtual Team? Team { get; set; }    
    [JsonIgnore]
    public virtual Service? Service { get; set; }
    [JsonIgnore]
    public virtual Category? Category { get; set; }
    [JsonIgnore]
    public virtual Mode? Mode { get; set; }
    [JsonIgnore]
    public virtual ICollection<Assignment>? Assignments { get; set; }
    [JsonIgnore]
    public virtual ICollection<TicketAnalyst>? TicketAnalysts { get; set; }
    [JsonIgnore]
    public virtual ICollection<TicketTask>? TicketTasks { get; set; }
    [JsonIgnore]
    public virtual ICollection<History>? Histories { get; set; }
}
