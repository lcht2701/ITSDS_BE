using API.DTOs.Requests.Contracts;
using API.DTOs.Responses.Contracts;
using Domain.Models.Contracts;

namespace API.Services.Interfaces
{
    public interface IContractService
    {
        Task<List<GetContractResponse>> Get();
        Task<List<GetContractResponse>> GetByCompanyAdmin(int userId);
        Task<GetContractResponse> GetById(int id, int currentUserId);
        Task<Contract> Create(CreateContractRequest model);
        Task<Contract> Update(int id, UpdateContractRequest model);
        Task Remove(int id);
    }
}
