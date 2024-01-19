using API.Mappings;
using Domain.Models.Tickets;

namespace API.DTOs.Requests.Teams
{
    public class CreateTeamRequest : IMapTo<Team>
    {
        public string Name { get; set; }

        public int ManagerId { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public int? CategoryId { get; set; }
    }
}
