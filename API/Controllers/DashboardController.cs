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
    [HttpGet("ticket/customer")]
    public async Task<IActionResult> GetCustomerTicketDashboard()
    {
        var dashboard = await _dashboardService.GetCustomerTicketDashboard(CurrentUserID);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.TECHNICIAN)]
    [HttpGet("ticket/technician")]
    public async Task<IActionResult> GetTechnicianTicketDashboard()
    {
        var dashboard = await _dashboardService.GetTechnicianTicketDashboard(CurrentUserID);
        return Ok(dashboard);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("ticket/manager")]
    public async Task<IActionResult> GetManagerTicketDashboard()
    {
        var dashboard = await _dashboardService.GetManagerTicketDashboard();
        return Ok(dashboard);
    }
}
