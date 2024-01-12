using API.DTOs.Requests.TicketSolutions;
using API.DTOs.Responses.TicketSolutions;

namespace API.Services.Interfaces
{
    public interface ITicketSolutionService
    {
        Task<List<GetTicketSolutionResponse>> Get(int userId);
        Task<List<GetTicketSolutionResponse>> GetUnapprovedSolutions(int userId);
        Task<GetTicketSolutionResponse> GetById(int id, int userId);
        Task Create(CreateTicketSolutionRequest model, int createdById);
        Task Update(int solutionId, UpdateTicketSolutionRequest model, int userId);
        Task Remove(int solutionId, int userId);
        Task Approve(int solutionId, ApproveSolutionRequest model);
        Task Reject(int solutionId);
        Task SubmitForApproval(int solutionId, int userId, SubmitApprovalRequest model);
    }
}
