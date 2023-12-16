using API.DTOs.Requests.Modes;
using Domain.Models.Tickets;

namespace API.Services.Interfaces
{
    public interface IModeService
    {
        Task<List<Mode>> Get();
        Task<Mode> GetById(int id);
        Task<Mode> Create(CreateModeRequest model);
        Task<Mode> Update(int id, UpdateModeRequest model);
        Task Remove(int id);
    }
}
