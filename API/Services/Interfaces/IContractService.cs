using API.DTOs.Requests.Contracts;
using Domain.Models.Contracts;

namespace API.Services.Interfaces
{
    public interface IContractService
    {
        Task<List<Contract>> Get();
        Task<List<Contract>> GetParentContracts();
        Task<List<Contract>> GetChildContracts(int contractId);
        Task<List<Contract>> GetByCustomer(int userId);
        Task<List<Contract>> GetByAccountant(int userId);
        Task<Contract> GetById(int id);
        Task<Contract> Create(CreateContractRequest model);
        Task<Contract> Update(int id, UpdateContractRequest model);
        Task Remove(int id);
        Task<List<Renewal>> GetContractRenewals(int contractId);
        Task<Renewal> RenewContract(int contractId, RenewContractRequest model, int userId);
    }
}
