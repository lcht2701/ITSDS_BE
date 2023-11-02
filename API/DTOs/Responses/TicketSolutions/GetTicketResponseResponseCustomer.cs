using Domain.Models;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.TicketSolutions
{
    public class GetTicketResponseResponseCustomer
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public int? CategoryId { get; set; }

        public int? OwnerId { get; set; }

        public string? Keyword { get; set; }

        public string? AttachmentUrl { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public virtual User Owner { get; set; }

        public virtual Category Category { get; set; }
    }
}
