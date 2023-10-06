using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Teams
{
    public class UpdateTeamRequest : IMapTo<Team>
    {
        public string? Name { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }

        public int? OwnerId { get; set; }
    }
}
