using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.TicketSolutions
{
    public class CreateTicketSolutionRequest : IMapTo<TicketSolution>
    {
        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        public int? OwnerId { get; set; }

        public DateTime ExpiredDate { get; set; }

        public string? Keyword { get; set; }

        public List<string>? AttachmentUrls { get; set; }
    }
}
