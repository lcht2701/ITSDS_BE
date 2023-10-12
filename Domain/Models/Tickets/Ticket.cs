using Domain.Constants.Enums;
using Domain.Models.Contracts;
using System.Text.Json.Serialization;

namespace Domain.Models.Tickets;

public partial class Ticket : BaseEntity
{
    public Ticket()
    {
        TicketTasks = new HashSet<TicketTask>();
        Histories = new HashSet<History>();
    }
    public int? RequesterId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? ModeId { get; set; }

    public int? ServiceId { get; set; }
    
    public int? CategoryId { get; set; }

    public TicketStatus? TicketStatus { get; set; }

    public Priority? Priority { get; set; }

    public DateTime? ScheduledStartTime { get; set; }

    public DateTime? ScheduledEndTime { get; set; }

    public DateTime? DueTime { get; set; }

    public DateTime? CompletedTime { get; set; }

    public Impact? Impact { get; set; }
    
    public string? ImpactDetail { get; set; }
    
    public Urgency? Urgency { get; set; }

    public string? AttachmentUrl { get; set; }

    public virtual User? Requester { get; set; }

    public virtual Assignment? Assignment { get; set; }  

    public virtual Service? Service { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Mode? Mode { get; set; }

    [JsonIgnore]
    public virtual ICollection<TicketTask>? TicketTasks { get; set; }
    [JsonIgnore]
    public virtual ICollection<History>? Histories { get; set; }
}
