using Domain.Constants.Enums;
using System.Text.Json.Serialization;

namespace Domain.Models.Tickets;

public partial class TicketTask : BaseEntity
{
    public int? TicketId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Note { get; set; }

    public int? TechnicianId { get; set; }

    public int? TeamId { get; set; }

    public Priority? Priority { get; set; }

    public DateTime? ScheduledStartTime { get; set; }

    public DateTime? ScheduledEndTime { get; set; }

    public DateTime? ActualStartTime { get; set; }

    public DateTime? ActualEndTime { get; set; }

    public int? Progress { get; set; }

    public double? AdditionalCost { get; set; }

    public string? AttachmentUrl { get; set; }

    public int? TimeSpent { get; set; }

    public DateTime? DateCompleted { get; set; }
    [JsonIgnore]
    public virtual User? Technician { get; set; }
    [JsonIgnore]
    public virtual Team? Team { get; set; }
    [JsonIgnore]
    public virtual Ticket? Ticket { get; set; }
}
