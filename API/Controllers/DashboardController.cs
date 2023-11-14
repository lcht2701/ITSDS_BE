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
}
