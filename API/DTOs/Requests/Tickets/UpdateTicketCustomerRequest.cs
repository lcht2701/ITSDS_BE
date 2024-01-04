using API.Mappings;
using Domain.Models.Tickets;

namespace API.DTOs.Requests.Tickets
{
    public class UpdateTicketCustomerRequest : IMapTo<Ticket>
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? ServiceId { get; set; }

        public List<string>? AttachmentUrls { get; set; }
    }
}
