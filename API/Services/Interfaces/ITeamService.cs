using API.DTOs.Requests.Teams;
using Domain.Models.Tickets;

namespace API.Services.Interfaces
{
    public interface ITeamService
    {
        Task<List<Team>> Get();
        Task<List<Team>> GetByManager(int managerId);
        Task<Team> GetById(int id);
        Task<Team> Create(CreateTeamRequest model);
        Task<Team> Update(int id, UpdateTeamRequest model);
        Task Remove(int id);
    }
}
