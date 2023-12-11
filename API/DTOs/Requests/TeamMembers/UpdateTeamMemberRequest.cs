using API.Mappings;
using Domain.Models.Tickets;

namespace API.DTOs.Requests.TeamMembers
{
    public class UpdateTeamMemberRequest : IMapTo<TeamMember>
    { 
        public string? Expertises { get; set; }

        public int? TeamId { get; set; }
    }
}
