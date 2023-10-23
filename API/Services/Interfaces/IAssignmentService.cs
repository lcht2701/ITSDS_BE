using API.DTOs.Requests.Assignments;
using Microsoft.AspNetCore.Mvc;
namespace API.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task<object> GetListOfTechnician(int? teamId);
        Task<object> Get();
        Task<object> GetByTechnician(int technicianId);
        Task<object> GetByTeam(int teamId);
        Task<object> GetById(int id);
        Task<IActionResult> Assign(int ticketId, AssignTicketManualRequest model);
        Task<IActionResult> Update(int ticketId, UpdateTicketAssignmentManualRequest model);
        Task Remove(int ticketId);
        Task<object> IsTechnicianMemberOfTeamAsync(int? technicianId, int? teamId);
        Task<int> FindTechnicianWithLeastAssignments(int? teamId);
        //Hangfire
        Task AssignSupportJob(int ticketId);
        Task CancelAssignSupportJob(string jobId, int ticketId);
    }
}
