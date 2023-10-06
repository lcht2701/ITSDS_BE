using API.Mappings;
using Domain.Constants;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Tickets
{
    public class CreateTicketCustomerRequest : IMapTo<Ticket>
    {
        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public Priority? Priority { get; set; }

        public int? CategoryId { get; set; }

        public string? AttachmentUrl { get; set; }
    }
}
