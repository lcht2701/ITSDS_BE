using API.DTOs.Requests.TeamMembers;
using Domain.Models;

namespace API.Services.Interfaces
{
    public interface ITeamMemberService
    {
        Task<List<User>> GetMembersNotInTeam(int teamId);
        Task<List<User>> GetByTeam(int teamId);
        Task Assign(AssignMemberToTeamRequest model);
        Task Update(int memberId, UpdateTeamMemberRequest model);
        Task Transfer(int memberId, int newTeamId);
        Task Remove(int memberId);
    }
}
