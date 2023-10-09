using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Assignments
{
    public class UpdateTicketAssignmentManualRequest : IMapTo<Assignment>
    {
        public int? TechnicianId { get; set; }

        public int? TeamId { get; set; }
    }
}
