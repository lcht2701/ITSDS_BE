using API.DTOs.Responses.Contracts;
using API.DTOs.Responses.Dashboards.Accountants;
using API.DTOs.Responses.Dashboards.Admins;
using API.DTOs.Responses.Dashboards.Customers;
using API.DTOs.Responses.Dashboards.Managers;
using API.DTOs.Responses.Dashboards.Managers.Tickets;
using API.DTOs.Responses.Dashboards.Technicians;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[Route("/v1/itsds/dashboard")]
public class DashboardController : BaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpGet("customer/ticket")]
    [SwaggerResponse(200, "Get Customer Ticket Dashboard", typeof(CustomerTicketDashboard))]
    public async Task<IActionResult> GetCustomerTicketDashboard()
    {
        var dashboard = await _dashboardService.GetCustomerTicketDashboard(CurrentUserID);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.TECHNICIAN)]
    [HttpGet("technician/ticket")]
    [SwaggerResponse(200, "Get Technician Ticket Dashboard", typeof(TechnicianTicketDashboard))]
    public async Task<IActionResult> GetTechnicianTicketDashboard()
    {
        var dashboard = await _dashboardService.GetTechnicianTicketDashboard(CurrentUserID);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket")]
    [SwaggerResponse(200, "Get Manager Ticket Dashboard", typeof(ManagerTicketDashboard))]
    public async Task<IActionResult> GetManagerTicketDashboard()
    {
        var dashboard = await _dashboardService.GetManagerTicketDashboard();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/contract")]
    [SwaggerResponse(200, "Get Manager Contract Dashboard", typeof(ManagerContractDashboard))]
    public async Task<IActionResult> GetManagerContractDashboard()
    {
        var dashboard = await _dashboardService.GetManagerContractDashboard();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/category")]
    [SwaggerResponse(200, "Get Manager Ticket Dashboard Table", typeof(ManagerTicketsDashboardTable))]
    public async Task<IActionResult> GetManagerTicketDashboardByCategory()
    {
        var dashboard = await _dashboardService.GetManagerTicketsByCategory();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/priority")]
    [SwaggerResponse(200, "Get Manager Ticket Dashboard Table By Priority", typeof(ManagerTicketsDashboardTable))]
    public async Task<IActionResult> GetManagerTicketDashboardByPriority()
    {
        var dashboard = await _dashboardService.GetManagerTicketsByPriority();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/mode")]
    [SwaggerResponse(200, "Get Manager Ticket Dashboard Table By Mode", typeof(ManagerTicketsDashboardTable))]
    public async Task<IActionResult> GetManagerTicketDashboardByMode()
    {
        var dashboard = await _dashboardService.GetManagerTicketsByMode();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/service")]
    [SwaggerResponse(200, "Get Manager Ticket Dashboard Table By Service", typeof(ManagerTicketsDashboardTable))]
    public async Task<IActionResult> GetManagerTicketDashboardByService()
    {
        var dashboard = await _dashboardService.GetManagerTicketsByService();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/this-week")]
    [SwaggerResponse(200, "Get Created Ticket Dashboard This Week", typeof(List<DashboardTableRow>))]
    public async Task<IActionResult> GetCreatedTicketThisWeek()
    {
        var dashboard = await _dashboardService.GetTicketDashboardByWeek(DateTime.Now);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/last-week")]
    [SwaggerResponse(200, "Get Created Ticket Dashboard Last Week", typeof(List<DashboardTableRow>))]
    public async Task<IActionResult> GetCreatedTicketLastWeek()
    {
        var dashboard = await _dashboardService.GetTicketDashboardByWeek(DateTime.Now.AddDays(-7));
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/this-month")]
    [SwaggerResponse(200, "Get Created Ticket Dashboard This Month", typeof(List<DashboardTableRow>))]
    public async Task<IActionResult> GetCreatedTicketThisMonth()
    {
        var dashboard = await _dashboardService.GetTicketDashboardByMonth(DateTime.Today);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/last-month")]
    [SwaggerResponse(200, "Get Created Ticket Dashboard Last Month", typeof(List<DashboardTableRow>))]
    public async Task<IActionResult> GetCreatedTicketLastMonth()
    {
        var dashboard = await _dashboardService.GetTicketDashboardByMonth(DateTime.Today.AddMonths(-1));
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/dashboard")]
    [SwaggerResponse(200, "Get Manager Dashboard", typeof(ManagerDashboard))]
    public async Task<IActionResult> GetManagerDashboard()
    {
        var dashboard = await _dashboardService.GetManagerDashboard();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("user/recent-created")]
    [SwaggerResponse(200, "Get Recently Created User", typeof(List<UserCreatedDashboardData>))]
    public async Task<IActionResult> GetRecentCreatedUser(int amount)
    {
        var dashboard = await _dashboardService.GetRecentCreatedUser(amount);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("user/recent-updated")]
    [SwaggerResponse(200, "Get Recently Updated User", typeof(List<UserUpdatedDashboardData>))]
    public async Task<IActionResult> GetRecentUpdatedUser(int amount)
    {
        var dashboard = await _dashboardService.GetRecentUpdatedUser(amount);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("user/active-count")]
    [SwaggerResponse(200, "Get Active User Count" , typeof(UserCreatedDashboardData))]
    public async Task<IActionResult> GetActiveUserCount()
    {
        var dashboard = await _dashboardService.GetActiveUserCount();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("user/role-count")]
    [SwaggerResponse(200, "Get User Role Count", typeof(UserRolesCountDashboard))]
    public async Task<IActionResult> GetUserRoleCount()
    {
        var dashboard = await _dashboardService.GetUserRoleCount();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("team/recent-created")]
    [SwaggerResponse(200, "Get Recent Created Team", typeof(List<TeamCreatedDashboardData>))]
    public async Task<IActionResult> GetRecentCreatedTeam(int amount)
    {
        var dashboard = await _dashboardService.GetRecentCreatedTeam(amount);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("team/recent-updated")]
    [SwaggerResponse(200, "Get Recent Updated Team", typeof(List<TeamUpdatedDashboardData>))]
    public async Task<IActionResult> GetRecentUpdatedTeam(int amount)
    {
        var dashboard = await _dashboardService.GetRecentUpdatedTeam(amount);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("team/active-count")]
    [SwaggerResponse(200, "Get Recent Created Team", typeof(TeamActiveDashboardData))]
    public async Task<IActionResult> GetActiveTeamCount()
    {
        var dashboard = await _dashboardService.GetActiveTeamCount();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("team/member-count")]
    [SwaggerResponse(200, "Get Team Member Count", typeof(TeamMemberDashboardData))]
    public async Task<IActionResult> GetTeamMemberCount()
    {
        var dashboard = await _dashboardService.GetTeamMemberCount();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ACCOUNTANT)]
    [HttpGet("accountant/contract")]
    [SwaggerResponse(200, "Get Accountant Dashboard", typeof(AccountantDashboard))]
    public async Task<IActionResult> GetAccountantDashboard()
    {
        var dashboard = await _dashboardService.GetAccountantDashboard(CurrentUserID);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ACCOUNTANT)]
    [HttpGet("accountant/contract/status")]
    [SwaggerResponse(200, "Get Accountant Contract Dashboard", typeof(AccountantContractDashboard))]
    public async Task<IActionResult> GetAccountantContractDashboard()
    {
        var dashboard = await _dashboardService.GetAccountantContractDashboard(CurrentUserID);
        return Ok(dashboard);
    }

}
