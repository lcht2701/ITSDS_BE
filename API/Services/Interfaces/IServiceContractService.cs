using API.DTOs.Requests.ServiceContracts;
using Domain.Models.Contracts;
using X.PagedList;

namespace API.Services.Interfaces
{
    public interface IServiceContractService
    {
        Task<List<ServiceContract>> Get(int contractId);
        Task<ServiceContract> GetById(int id);
        Task<List<ServiceContract>> ModifyServices(ModifyServicesInContract model);
    }
}
