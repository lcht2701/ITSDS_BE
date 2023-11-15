using API.DTOs.Responses.Dashboards.Admins;
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
    Task<List<DashboardTableRow>> GetTicketDashboardByWeek(DateTime currentDate);
    Task<List<DashboardTableRow>> GetTicketDashboardByMonth(DateTime currentDate);
    //Assignment

    //Ticket Task

    //Contract

    //User
    Task<List<UserCreatedDashboardData>> GetRecentCreatedUser(int amount);
    Task<List<UserUpdatedDashboardData>> GetRecentUpdatedUser(int amount);
    Task<UserActiveDashboardData> GetActiveUserCount();
    Task<UserRolesCountDashboard> GetUserRoleCount();

    //Team
    Task<TeamActiveDashboardData> GetActiveTeamCount();
    Task<TeamMemberDashboardData> GetTeamMemberCount();
    Task<List<TeamCreatedDashboardData>> GetRecentCreatedTeam(int amount);
    Task<List<TeamUpdatedDashboardData>> GetRecentUpdatedTeam(int amount);

}