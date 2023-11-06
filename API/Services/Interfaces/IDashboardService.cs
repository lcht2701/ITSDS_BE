using API.DTOs.Responses.Dashboards.Tickets;

namespace API.Services.Interfaces;

public interface IDashboardService
{
    //Ticket
    Task<CustomerTicketModel> GetCustomerTicketDashboard(int userId);
    Task<TechnicianTicketModel> GetTechnicianTicketDashboard(int userId);
    Task<ManagerTicketModel> GetManagerTicketDashboard();
    //Assignment

    //Ticket Task

    //Contract
}
