using API.DTOs.Requests.TicketSolutions;
using API.DTOs.Responses.TicketSolutions;

namespace API.Services.Interfaces
{
    public interface ITicketSolutionService
    {
        Task<List<GetTicketSolutionResponse>> Get(int userId);
        Task<object> GetById(int id);
        Task Create(CreateTicketSolutionRequest model);
        Task Update(int solutionId, UpdateTicketSolutionRequest model);
        Task Remove(int solutionId);
        Task Approve(int solutionId);
        Task Reject(int solutionId);
        Task SubmitForApproval(int solutionId);
        Task ChangePublic(int solutionId);
    }
}
