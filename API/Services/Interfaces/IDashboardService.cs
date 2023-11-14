using API.DTOs.Responses.Dashboards.Customers;
using API.DTOs.Responses.Dashboards.Managers.Tickets;
using API.DTOs.Responses.Dashboards.Technicians;

namespace API.Services.Interfaces;

public interface IDashboardService
{
    //Ticket
    Task<CustomerTicketDashboard> GetCustomerTicketDashboard(int userId);
    Task<TechnicianTicketDashboard> GetTechnicianTicketDashboard(int userId);
    Task<ManagerTicketDashboard> GetManagerTicketDashboard();
    Task<ManagerTicketsDashboardTable> GetManagerTicketsByCategory();
    Task<ManagerTicketsDashboardTable> GetManagerTicketsByPriority();
    Task<ManagerTicketsDashboardTable> GetManagerTicketsByMode();
    Task<ManagerTicketsDashboardTable> GetManagerTicketsByService();
    //Assignment

    //Ticket Task

    //Contract
}