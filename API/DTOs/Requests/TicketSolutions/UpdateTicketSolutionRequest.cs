using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.TicketSolutions
{
    public class UpdateTicketSolutionRequest : IMapTo<TicketSolution>
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int CategoryId { get; set; }

        public int OwnerId { get; set; }

        public DateTime ReviewDate { get; set; }

        public DateTime ExpiredDate { get; set; }

        public string Keyword { get; set; }

        public string InternalComments { get; set; }

        public bool? IsPublic { get; set; }
    }
}
