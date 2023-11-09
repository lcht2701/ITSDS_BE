using Domain.Models.Contracts;

namespace API.Services.Interfaces
{
    public interface IServiceServicePackService
    {
        Task<List<Service>> GetServices(int packId);
        Task AddService(int packId, List<int> serviceIds);
        Task RemoveService(int id);
    }
}
