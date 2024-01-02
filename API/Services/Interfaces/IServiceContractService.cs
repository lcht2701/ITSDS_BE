using API.DTOs.Requests.ServiceContracts;
using Domain.Models.Contracts;
using Domain.Models.Tickets;

namespace API.Services.Interfaces
{
    public interface IServiceContractService
    {
        Task<List<ServiceContract>> Get(int contractId);
        Task<List<Service>> GetActiveServicesOfMemberCompany(int userId);
        Task<ServiceContract> GetById(int id);
        Task<List<ServiceContract>> ModifyServices(ModifyServicesInContract model);
        Task<List<Service>> GetServicesList(int contractId);
        Task<List<ServiceContract>> Add(int contractId, List<int> serviceIds);
        Task<ServiceContract> AddPeriodicService(int contractId, AddPeriodicService model);
        Task Remove(int id);
        Task<List<Ticket>> CreatePeriodicTickets(int id, int currentUserId);
    }
}
