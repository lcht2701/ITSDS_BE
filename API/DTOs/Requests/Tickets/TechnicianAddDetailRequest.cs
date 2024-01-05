using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;

namespace API.DTOs.Requests.Tickets
{
    public class TechnicianAddDetailRequest : IMapTo<Ticket>
    {
        public string? Type { get; set; }

        public Priority? Priority { get; set; }
        
        public Impact? Impact { get; set; }

        public string? ImpactDetail { get; set; }

        public DateTime? ScheduledStartTime { get; set; }

        public DateTime? ScheduledEndTime { get; set; }
    }
}
