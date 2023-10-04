using API.Mappings;
using Domain.Constants;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Tickets
{
    public class CreateTicketRequest : IMapTo<Ticket>
    {
        [Required]
        public int RequesterId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int ServiceId { get; set; }

        public string Description { get; set; }

        public string AttachmentUrl { get; set; }

        public string RequesterNote { get; set; }
    }
}
