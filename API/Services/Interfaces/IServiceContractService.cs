using API.DTOs.Requests.ServiceContracts;
using Domain.Models.Contracts;

namespace API.Services.Interfaces
{
    public interface IServiceContractService
    {
        Task<List<ServiceContract>> Get(int contractId);
        Task<List<Service>> GetActiveServicesOfMemberCompany(int userId);
        Task<ServiceContract> GetById(int id);
        Task<List<Service>> GetServicesList(int contractId);
        Task<List<ServiceContract>> AddAndUpdate(int contractId, List<int> serviceIds);
        Task Remove(int contractId);
    }
}