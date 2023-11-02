using API.Mappings;
using Domain.Models;
using Domain.Models.Tickets;
using System.Text.Json.Serialization;

namespace API.DTOs.Responses.Assignments
{
    public class GetAssignmentResponse : IMapFrom<Assignment>
    {
        public int Id { get; set; }

        public int? TechnicianId { get; set; }

        public int? TeamId { get; set; }

        public int? TicketId { get; set; }

        public string? TechnicianFullName => $"{Technician?.FirstName} {Technician?.LastName}";

        public string? TechnicianEmail => Technician?.Email;

        public string? TeamName => Team?.Name;

        [JsonIgnore]
        public virtual User? Technician { get; set; }
        [JsonIgnore]
        public virtual Team? Team { get; set; }
        [JsonIgnore]
        public virtual Ticket? Ticket { get; set; }
    }

}
