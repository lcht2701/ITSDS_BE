using API.Mappings;
using Domain.Constants;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.TeamMembers
{
    public class AddMemberToTeamRequest : IMapTo<TeamMember>
    {
        public int MemberId { get; set; }

        public int TeamId { get; set; }

        public string Expertises { get; set; }
    }
}
