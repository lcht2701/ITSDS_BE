using API.DTOs.Requests.Tickets;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers;

[Route("/v1/itsds/ticket")]
public class TicketController : BaseController
{
    private readonly IAuditLogService _auditLogService;
    private readonly ITicketService _ticketService;

    public TicketController(IAuditLogService auditLogService, ITicketService ticketService)
    {
        _auditLogService = auditLogService;
        _ticketService = ticketService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTickets(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var response = await _ticketService.Get();
        var pagedResponse = response.AsQueryable().GetPagedData(page, pageSize, filter, sort);

        return Ok(pagedResponse);
    }


    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTicketsOfUser(int userId,
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        try
        {
            var response = await _ticketService.GetByUser(userId);
            var pagedResponse = response.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpGet("user/history")]
    public async Task<IActionResult> GetTicketHistory()
    {
        try
        {
            var response = await _ticketService.GetTicketHistory(CurrentUserID);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpGet("user/available")]
    public async Task<IActionResult> GetAvailableTicketsOfCurrentUser()
    {
        try
        {
            var response = await _ticketService.GetTicketAvailable(CurrentUserID);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.TECHNICIAN)]
    [HttpGet("assign")]
    public async Task<IActionResult> GetAssignedTickets()
    {
        try
        {
            var response = await _ticketService.GetAssignedTickets(CurrentUserID);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetTicketById(int ticketId)
    {
        try
        {
            var result = await _ticketService.GetById(ticketId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Ticket is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("log")]
    public async Task<IActionResult> GetTicketLog(int ticketId)
    {
        try
        {
            var logs = await _ticketService.GetTicketLog(ticketId);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpPost("customer/new")]
    public async Task<IActionResult> CreateTicketByCustomer([FromBody] CreateTicketCustomerRequest model)
    {
        try
        {
            var entity = await _ticketService.CreateByCustomer(CurrentUserID, model);
            await _auditLogService.TrackCreated(entity.Id, Tables.TICKET, CurrentUserID);
            return Ok("Create Successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpPut("customer/{ticketId}")]
    public async Task<IActionResult> UpdateTicketByCustomer(int ticketId, [FromBody] UpdateTicketCustomerRequest model)
    {
        try
        {
            var original = await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            var updated = await _ticketService.UpdateByCustomer(ticketId, model);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            return Ok("Update Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Ticket is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }



    [Authorize(Roles = Roles.MANAGER)]
    [HttpPost("manager/new")]
    public async Task<IActionResult> CreateTicketByManager([FromBody] CreateTicketManagerRequest model)
    {
        try
        {
            Ticket entity = await _ticketService.CreateByManager(model);
            await _auditLogService.TrackCreated(entity.Id, Tables.TICKET, CurrentUserID);
            return Ok("Created Successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPut("manager/{ticketId}")]
    public async Task<IActionResult> UpdateTicketByManager(int ticketId, [FromBody] UpdateTicketManagerRequest model)
    {
        try
        {
            var original = await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            var updated = await _ticketService.UpdateByManager(ticketId, model);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            return Ok("Update Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Ticket is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpDelete("manager/{ticketId}")]
    public async Task<IActionResult> DeleteTicket(int ticketId)
    {
        try
        {
            await _ticketService.Remove(ticketId);
            return Ok("Removed Successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}