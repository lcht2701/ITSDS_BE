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
        Feedbacks = new HashSet<Feedback>();
    }
    public int RequesterId { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public int ServiceId { get; set; } 

    public Service Service { get; set; }

    public string? AttachmentUrl { get; set; }

    public TicketStatus TicketStatus { get; set; }

    public Priority? Priority { get; set; }

    public DateTime? EstimatedFinishTime { get; set; }

    public DateTime? ActualFinishTime { get; set; }

    public string? RequesterNote { get; set; }

    public string? TechnicianNote { get; set; }

    public int? TeamId { get; set; }

    public virtual Team Team { get; set; }

    [JsonIgnore]
    public virtual ICollection<Assignment> Assignments { get; set; }

    [JsonIgnore]
    public virtual ICollection<TicketTask> TicketTasks { get; set; }

    [JsonIgnore]
    public virtual ICollection<TicketApproval> TicketApprovals { get; set; }

    [JsonIgnore]
    public virtual ICollection<Feedback> Feedbacks { get; set; }


}
