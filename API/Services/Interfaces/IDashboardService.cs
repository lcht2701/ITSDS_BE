using API.DTOs.Responses.Dashboards.Customers;
using API.DTOs.Responses.Dashboards.Managers.Tickets;
using API.DTOs.Responses.Dashboards.Technicians;
using Domain.Models.Tickets;

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
    Task<List<DashboardTableRow>> GetTicketDashboardByWeek(DateTime currentDate);
    Task<List<DashboardTableRow>> GetTicketDashboardByMonth(DateTime currentDate);
    //Assignment

    //Ticket Task

    //Contract
}