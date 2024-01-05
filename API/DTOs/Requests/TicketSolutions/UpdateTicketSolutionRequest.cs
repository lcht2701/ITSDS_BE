using API.Mappings;
using Domain.Models.Tickets;

namespace API.DTOs.Requests.TicketSolutions
{
    public class UpdateTicketSolutionRequest : IMapTo<TicketSolution>
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int CategoryId { get; set; }

        public int OwnerId { get; set; }

        public DateTime ExpiredDate { get; set; }

        public string Keyword { get; set; }

        public bool? IsPublic { get; set; }

        public List<string>? AttachmentUrls { get; set; }
    }
}
