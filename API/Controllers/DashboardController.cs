using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetCustomerTicketDashboard()
    {
        var dashboard = await _dashboardService.GetCustomerTicketDashboard(CurrentUserID);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.TECHNICIAN)]
    [HttpGet("technician/ticket")]
    public async Task<IActionResult> GetTechnicianTicketDashboard()
    {
        var dashboard = await _dashboardService.GetTechnicianTicketDashboard(CurrentUserID);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket")]
    public async Task<IActionResult> GetManagerTicketDashboard()
    {
        var dashboard = await _dashboardService.GetManagerTicketDashboard();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/contract")]
    public async Task<IActionResult> GetManagerContractDashboard()
    {
        var dashboard = await _dashboardService.GetManagerContractDashboard();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/category")]
    public async Task<IActionResult> GetManagerTicketDashboardByCategory()
    {
        var dashboard = await _dashboardService.GetManagerTicketsByCategory();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/priority")]
    public async Task<IActionResult> GetManagerTicketDashboardByPriority()
    {
        var dashboard = await _dashboardService.GetManagerTicketsByPriority();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/mode")]
    public async Task<IActionResult> GetManagerTicketDashboardByMode()
    {
        var dashboard = await _dashboardService.GetManagerTicketsByMode();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/service")]
    public async Task<IActionResult> GetManagerTicketDashboardByService()
    {
        var dashboard = await _dashboardService.GetManagerTicketsByService();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/this-week")]
    public async Task<IActionResult> GetCreatedTicketThisWeek()
    {
        var dashboard = await _dashboardService.GetTicketDashboardByWeek(DateTime.Now);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/last-week")]
    public async Task<IActionResult> GetCreatedTicketLastWeek()
    {
        var dashboard = await _dashboardService.GetTicketDashboardByWeek(DateTime.Now.AddDays(-7));
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/this-month")]
    public async Task<IActionResult> GetCreatedTicketThisMonth()
    {
        var dashboard = await _dashboardService.GetTicketDashboardByMonth(DateTime.Now);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("manager/ticket/last-month")]
    public async Task<IActionResult> GetCreatedTicketLastMonth()
    {
        var dashboard = await _dashboardService.GetTicketDashboardByMonth(DateTime.Now.AddMonths(-1));
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("user/recent-created")]
    public async Task<IActionResult> GetRecentCreatedUser(int amount)
    {
        var dashboard = await _dashboardService.GetRecentCreatedUser(amount);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("user/recent-updated")]
    public async Task<IActionResult> GetRecentUpdatedUser(int amount)
    {
        var dashboard = await _dashboardService.GetRecentUpdatedUser(amount);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("user/active-count")]
    public async Task<IActionResult> GetActiveUserCount()
    {
        var dashboard = await _dashboardService.GetActiveUserCount();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("user/role-count")]
    public async Task<IActionResult> GetUserRoleCount()
    {
        var dashboard = await _dashboardService.GetUserRoleCount();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("team/recent-created")]
    public async Task<IActionResult> GetRecentCreatedTeam(int amount)
    {
        var dashboard = await _dashboardService.GetRecentCreatedTeam(amount);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("team/recent-updated")]
    public async Task<IActionResult> GetRecentUpdatedTeam(int amount)
    {
        var dashboard = await _dashboardService.GetRecentUpdatedTeam(amount);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("team/active-count")]
    public async Task<IActionResult> GetActiveTeamCount()
    {
        var dashboard = await _dashboardService.GetActiveTeamCount();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet("team/member-count")]
    public async Task<IActionResult> GetTeamMemberCount()
    {
        var dashboard = await _dashboardService.GetTeamMemberCount();
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ACCOUNTANT)]
    [HttpGet("accountant/contract")]
    public async Task<IActionResult> GetAccountantDashboard()
    {
        var dashboard = await _dashboardService.GetAccountantDashboard(CurrentUserID);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.ACCOUNTANT)]
    [HttpGet("accountant/contract/status")]
    public async Task<IActionResult> GetAccountantContractDashboard()
    {
        var dashboard = await _dashboardService.GetAccountantContractDashboard(CurrentUserID);
        return Ok(dashboard);
    }

}
