using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System.ComponentModel;

namespace API.DTOs.Requests.Tickets
{
    public class CreateTicketManagerRequest : IMapTo<Ticket>
    {
        public int? RequesterId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? ModeId { get; set; }

        public int? CategoryId { get; set; }
        
        public int? ServiceId { get; set; }

        public string? Type { get; set; }

        public Priority? Priority { get; set; }

        public Impact? Impact { get; set; }

        public string? ImpactDetail { get; set; }

        public List<string>? AttachmentUrls { get; set; }

        public DateTime? ScheduledStartTime { get; set; }

        public DateTime? ScheduledEndTime { get; set; }

        [DefaultValue(null)]
        public int? TeamId { get; set; }

        [DefaultValue(null)]
        public int? TechnicianId { get; set; }
    }
}
