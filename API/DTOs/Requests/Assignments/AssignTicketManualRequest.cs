using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Assignments
{
    public class AssignTicketManualRequest : IMapTo<Assignment>
    {
        [Required]
        public int TechnicianId { get; set; }

        [Required]
        public int TeamId { get; set; }
    }
}
