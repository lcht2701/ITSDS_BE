using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;

namespace API.DTOs.Requests.Tickets
{
    public class TechnicianAddDetailRequest : IMapTo<Ticket>
    {
        public Impact? Impact { get; set; }

        public string? ImpactDetail { get; set; }

        public Urgency? Urgency { get; set; }
    }
}
