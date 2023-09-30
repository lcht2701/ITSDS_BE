using API.Mappings;
using Domain.Constants;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.TeamMembers
{
    public class AssignMemberToTeamRequest : IMapTo<TeamMember>
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int TeamId { get; set; }

        public string Expertises { get; set; }
    }
}
