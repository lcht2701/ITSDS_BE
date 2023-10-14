using Domain.Constants.Enums;

namespace Domain.Models.Tickets;

public partial class TicketTask : BaseEntity
{
    public int? TicketId { get; set; }

    public int? CreateById { get; set; }

    public int? TechnicianId { get; set; }

    public int? TeamId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Note { get; set; }

    public Priority? Priority { get; set; }

    public DateTime? ScheduledStartTime { get; set; }

    public DateTime? ScheduledEndTime { get; set; }

    public DateTime? ActualStartTime { get; set; }

    public DateTime? ActualEndTime { get; set; }

    public int? Progress { get; set; }

    public TicketTaskStatus? TaskStatus { get; set; }

    public string? AttachmentUrl { get; set; }

    public virtual User? Technician { get; set; }

    public virtual User? CreateBy { get; set; }

    public virtual Team? Team { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
