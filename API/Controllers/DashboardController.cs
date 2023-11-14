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
}
