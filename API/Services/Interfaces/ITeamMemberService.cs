using API.DTOs.Requests.TeamMembers;
using Domain.Models;
using Domain.Models.Tickets;

namespace API.Services.Interfaces
{
    public interface ITeamMemberService
    {
        Task<List<TeamMember>> Get();
        Task<TeamMember> GetById(int id);
        Task<List<User>> GetMembersNotInTeam(int teamId);
        Task<List<User>> GetMembersInTeam(int teamId);
        Task Add(AddMemberToTeamRequest model);
        Task Update(int id, UpdateTeamMemberRequest model);
        Task Remove(int id);
    }
}
