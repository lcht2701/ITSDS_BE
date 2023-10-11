using System.ComponentModel;
using API.Mappings;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.Tickets;

public class GetTicketResponse : IMapFrom<Ticket>
{
    public int? Id { get; set; }
    public int? RequesterId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? ModeId { get; set; }
    public int? ServiceId { get; set; }
    public int? CategoryId { get; set; }
    public string? TicketStatus { get; set; }
    public string? Priority { get; set; }
    public string? Impact { get; set; }
    public string? ImpactDetail { get; set; }
    public string? Urgency { get; set; }
    public string? AttachmentUrl { get; set; }
    public DateTime? ScheduledStartTime { get; set; }
    public DateTime? ScheduledEndTime { get; set; }
    public DateTime? DueTime { get; set; }
    public DateTime? CompletedTime { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}