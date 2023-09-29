using API.Mappings;
using Domain.Constants;
using Domain.Models.Tickets;

namespace API.DTOs.Tickets.Requests
{
    public class UpdateTicketRequest : IMapTo<Ticket>
    {

        public TicketStatus TicketStatus { get; set; }

        public string? Note { get; set; }

        public DateTime EndDate { get; set; }

        public int ServiceId { get; set; }
    }
}
