using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Tickets
{
    public class CreateTicketManagerRequest : IMapTo<Ticket>
    {
        public int? RequesterId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? ModeId { get; set; }

        public int? ServiceId { get; set; }

        public int? CategoryId { get; set; }

        public TicketStatus? TicketStatus { get; set; }

        public Priority? Priority { get; set; }

        public Impact? Impact { get; set; }

        public string? ImpactDetail { get; set; }

        public Urgency? Urgency { get; set; }

        public string? AttachmentUrl { get; set; }
    }
}
