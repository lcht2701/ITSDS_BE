using API.DTOs.Requests.Assignments;
using System.Text.Json.Serialization;

namespace API.DTOs.Requests.Tickets
{
    public class CreateTicketManagerRequest
    {
        public CreateTicketManagerDetail? Ticket { get; set; }
        public AssignTicketManualRequest? Assignment { get; set; }

    }
}
