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
    Task<object> GetTicketLog(int id);
    Task<object> GetById(int id);
    Task<Ticket> CreateByCustomer(int userId, CreateTicketCustomerRequest model);
    Task<Ticket> UpdateByCustomer(int id, UpdateTicketCustomerRequest model);
    Task<Ticket> CreateByManager(CreateTicketManagerRequest model);
    Task<Ticket> UpdateByManager(int id, UpdateTicketManagerRequest model);
    Task Remove(int id);
    Task<bool> IsTicketAssigned (int ticketId);
    bool IsTicketDone(Ticket ticket);
    Task UpdateTicketStatus(int ticketId, TicketStatus newStatus);
    Task UpdateTicketStatusForTechnician(int ticketId, TicketStatus newStatus);
}
