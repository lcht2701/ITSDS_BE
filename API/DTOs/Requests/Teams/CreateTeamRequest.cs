using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Teams
{
    public class CreateTeamRequest : IMapTo<Team>
    {
        [Required]
        public string Name { get; set; }

        public int ManagerId { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }
    }
}
