using API.Mappings;
using Domain.Constants;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Tickets
{
    public class UpdateTicketManagerRequest : IMapTo<Ticket>
    {
        [Required]
        public int? RequesterId { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? ModeId { get; set; }

        public int? ServiceId { get; set; }

        public int? TeamId { get; set; }

        public TicketStatus? TicketStatus { get; set; }

        public Priority? Priority { get; set; }

        public Impact? Impact { get; set; }

        public Urgency? Urgency { get; set; }

        public int? CategoryId { get; set; }

        public string? AttachmentUrl { get; set; }

        public DateTime? ScheduledStartTime { get; set; }

        public DateTime? ScheduledEndTime { get; set; }

        public DateTime? DueTime { get; set; }

        public DateTime? CompletedTime { get; set; }
    }
}
