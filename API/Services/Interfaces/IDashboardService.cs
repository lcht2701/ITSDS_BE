using API.DTOs.Responses.Dashboards.Tickets;

namespace API.Services.Interfaces;

public interface IDashboardService
{
    //Ticket
    Task<CustomerTicketDashboard> GetCustomerTicketDashboard(int userId);
    Task<TechnicianTicketDashboard> GetTechnicianTicketDashboard(int userId);
    Task<ManagerTicketDashboard> GetManagerTicketDashboard();
    //Assignment

    //Ticket Task

    //Contract
}
