using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.TicketAnalysts
{
    public class UpdateTicketAnalystRequest : IMapTo<TicketAnalyst>
    {
        public string Impact { get; set; }

        public string RootCause { get; set; }

        public string Symptoms { get; set; }

        public string Attachments { get; set; }
    }
}
