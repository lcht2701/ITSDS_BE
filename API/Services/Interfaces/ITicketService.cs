using API.DTOs.Requests.Tickets;
using API.DTOs.Responses.Tickets;
using Domain.Constants.Enums;
using Domain.Models.Tickets;

namespace API.Services.Interfaces;

public interface ITicketService
{
    Task<List<GetTicketResponse>> Get();
    Task<List<GetTicketResponse>> GetByUser(int userId);
    Task<List<GetTicketResponse>> GetTicketHistory(int userId);
    Task<List<GetTicketResponse>> GetTicketAvailable(int userId);
    Task<List<GetTicketResponse>> GetAssignedTickets(int userId);
    Task<List<GetTicketResponse>> GetCompletedAssignedTickets(int userId);
    Task<List<GetTicketStatusesResponse>> GetTicketStatuses();
    Task<object> GetTicketLog(int id);
    Task<GetTicketResponse> GetById(int id);
    Task<Ticket> CreateByCustomer(int createdBy, CreateTicketCustomerRequest model);
    Task<Ticket> UpdateByCustomer(int id, UpdateTicketCustomerRequest model);
    Task<Ticket> CreateByManager(int createdBy, CreateTicketManagerRequest model);
    Task<Ticket> UpdateByTechnician(int id, TechnicianAddDetailRequest model);
    Task<Ticket> UpdateByManager(int id, UpdateTicketManagerRequest model);
    Task Remove(int id);
    Task<bool> IsTicketAssigned(int ticketId);
    bool IsTicketDone(int ticketId);
    Task<Ticket> UpdateTicketStatus(int ticketId, TicketStatus newStatus);
    Task<Ticket> ModifyTicketStatus(int ticketId, TicketStatus newStatus);
    Task<Ticket> CancelTicket(int ticketId, int userId);
    Task<Ticket> CloseTicket(int ticketId, int userId);
    //Background jobs
    Task AssignSupportJob(int ticketId);
    Task<bool> CancelAssignSupportJob(string jobId, int ticketId);
    Task CloseTicketJob(int ticketId);
    Task CancelCloseTicketJob(string jobId, int ticketId);
}
