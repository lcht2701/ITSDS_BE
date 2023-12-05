using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System.ComponentModel;

namespace API.DTOs.Requests.Tickets
{
    public class CreateTicketManagerDetail : IMapTo<Ticket>
    {
        public int? RequesterId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? ModeId { get; set; }

        public int? CategoryId { get; set; }
        
        public int? ServiceId { get; set; }

        [DefaultValue(false)]
        public bool IsPeriodic { get; set; }

        public string? Type { get; set; }

        public string? Street { get; set; }

        public int? Ward { get; set; }

        public int? District { get; set; }

        public int? City { get; set; }

        public TicketStatus? TicketStatus { get; set; }

        public Priority? Priority { get; set; }

        public Impact? Impact { get; set; }

        public string? ImpactDetail { get; set; }

        public Urgency? Urgency { get; set; }

        public string? AttachmentUrl { get; set; }
    }
}
