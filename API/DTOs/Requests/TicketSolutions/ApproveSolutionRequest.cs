using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.TicketSolutions
{
    public class ApproveSolutionRequest : IMapTo<TicketSolution>
    {
        public int Duration { get; set; }
    }
}
