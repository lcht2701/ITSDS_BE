using API.DTOs.Requests.TicketTasks;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Constants.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers;

[Route("/v1/itsds/ticket/task")]
public class TicketTaskController : BaseController
{
    private readonly ITicketTaskService _ticketTaskService;

    public TicketTaskController(ITicketTaskService ticketTaskService)
    {
        _ticketTaskService = ticketTaskService;
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet]
    public async Task<IActionResult> GetAllTasksOfTicket(int ticketId,
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        try
        {
            var result = await _ticketTaskService.Get(ticketId);
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPost("new")]
    public async Task<IActionResult> CreateTask(int ticketId, [FromBody] CreateTicketTaskRequest model)
    {
        try
        {
            await _ticketTaskService.Create(ticketId, model, CurrentUserID);
            return Ok("Create Successfully");
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTicketTask(int taskId, [FromBody] UpdateTicketTaskRequest req)
    {
        try
        {
            await _ticketTaskService.Update(taskId, req);
            return Ok("Updated Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Task is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTicketTask(int taskId)
    {
        try
        {
            await _ticketTaskService.Remove(taskId);
            return Ok("Deleted Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Task is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPatch("update-status")]
    public async Task<IActionResult> UpdateTaskStatus(int taskId, TicketTaskStatus newStatus)
    {
        try
        {
            await _ticketTaskService.UpdateTaskStatus(taskId, newStatus);
            return Ok("Update Status Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Task is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
