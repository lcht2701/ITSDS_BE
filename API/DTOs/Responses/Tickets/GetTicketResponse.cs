using System.ComponentModel;
using API.DTOs.Responses.Assignments;
using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.Tickets;

public class GetTicketResponse : IMapFrom<Ticket>
{
    public int Id { get; set; }
    public int? RequesterId { get; set; }
    public int? CreatedById { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Street { get; set; }
    public int? Ward { get; set; }
    public int? District { get; set; }
    public int? City { get; set; }
    public int? ModeId { get; set; }
    public int? ServiceId { get; set; }
    public int? CategoryId { get; set; }
    public TicketStatus? TicketStatus { get; set; }
    public Priority? Priority { get; set; }
    public Impact? Impact { get; set; }
    public string? ImpactDetail { get; set; }
    public Urgency? Urgency { get; set; }
    public List<string>? AttachmentUrl { get; set; }
    public DateTime? ScheduledStartTime { get; set; }
    public DateTime? ScheduledEndTime { get; set; }
    public DateTime? DueTime { get; set; }
    public DateTime? CompletedTime { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public GetAssignmentResponse? Assignment { get; set; }
    public virtual User? Requester { get; set; }
    public virtual User? CreatedBy { get; set; }
    public virtual Service? Service { get; set; }
    public virtual Category? Category { get; set; }
    public virtual Mode? Mode { get; set; }
}