using API.DTOs.Requests.ServicePacks;
using Domain.Models.Contracts;

namespace API.Services.Interfaces
{
    public interface IServicePackService
    {
        Task<List<ServicePack>> Get();
        Task<ServicePack> GetById(int id);
        Task<ServicePack> Create(CreateServicePackRequest model);
        Task<ServicePack> Update(int id, UpdateServicePackRequest model);
        Task Remove(int id);
    }
}
