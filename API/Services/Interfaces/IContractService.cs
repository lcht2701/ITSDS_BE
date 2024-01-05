using API.DTOs.Requests.Contracts;
using API.DTOs.Responses.Contracts;
using Domain.Models.Contracts;
using System.Collections.Generic;

namespace API.Services.Interfaces
{
    public interface IContractService
    {
        Task<List<GetContractResponse>> Get();
        Task<List<GetContractResponse>> GetParentContracts();
        Task<List<GetContractResponse>> GetChildContracts(int contractId);
        Task<List<GetContractResponse>> GetByCompanyAdmin(int userId);
        Task<List<GetContractResponse>> GetByAccountant(int userId);
        Task<GetContractResponse> GetById(int id, int currentUserId);
        Task<Contract> Create(CreateContractRequest model);
        Task<Contract> Update(int id, UpdateContractRequest model);
        Task Remove(int id);
        Task<List<Renewal>> GetContractRenewals(int contractId);
        Task<Renewal> RenewContract(int contractId, RenewContractRequest model, int userId);
    }
}
