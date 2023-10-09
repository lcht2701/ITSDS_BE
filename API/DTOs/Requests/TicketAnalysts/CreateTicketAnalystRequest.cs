using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.TicketAnalysts
{
    public class CreateTicketAnalystRequest : IMapTo<TicketAnalyst>
    {
        [Required]
        public string Impact { get; set; }

        [Required]
        public string RootCause { get; set; }

        [Required]
        public string Symptoms { get; set; }

        public string Attachments { get; set; }
    }
}
