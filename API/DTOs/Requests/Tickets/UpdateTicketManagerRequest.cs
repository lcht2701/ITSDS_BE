using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Tickets
{
    public class UpdateTicketManagerRequest : IMapTo<Ticket>
    {
        public int? RequesterId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? ModeId { get; set; }

        public TicketStatus? TicketStatus { get; set; }

        public Priority? Priority { get; set; }

        public Impact? Impact { get; set; }

        public string? ImpactDetail { get; set; }

        public Urgency? Urgency { get; set; }

        public int? CategoryId { get; set; }

        public int? ServiceId { get; set; }

        public bool? IsPeriodic { get; set; }

        public string? Type { get; set; }

        public string? Street { get; set; }

        public int? Ward { get; set; }

        public int? District { get; set; }

        public int? City { get; set; }

        public List<string>? AttachmentUrls { get; set; }

        public DateTime? ScheduledStartTime { get; set; }

        public DateTime? ScheduledEndTime { get; set; }

        public DateTime? DueTime { get; set; }

        public DateTime? CompletedTime { get; set; }
    }
}
