using API.Mappings;
using Domain.Constants;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.TeamMembers
{
    public class UpdateTeamMemberRequest : IMapTo<TeamMember>
    { 
        public string Expertises { get; set; }
    }
}
