using API.DTOs.Requests.TicketTasks;
using API.DTOs.Responses.TicketTasks;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[Route("/v1/itsds/ticket/task")]
public class TicketTaskController : BaseController
{
    private readonly ITicketTaskService _ticketTaskService;
    private readonly IMessagingService _messagingService;
    private readonly IRepositoryBase<Ticket> _ticketRepository;

    public TicketTaskController(ITicketTaskService ticketTaskService, IMessagingService messagingService, IRepositoryBase<Ticket> ticketRepository)
    {
        _ticketTaskService = ticketTaskService;
        _messagingService = messagingService;
        _ticketRepository = ticketRepository;
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

    [Authorize(Roles = Roles.TECHNICIAN)]
    [HttpGet("active")]
    [SwaggerResponse(200, "Get Active Tasks", typeof(List<GetTicketTaskResponse>))]
    public async Task<IActionResult> GetActiveTasks([FromQuery] int? ticketId)
    {
        try
        {
            var result = await _ticketTaskService.GetActiveTasks(CurrentUserID, ticketId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.TECHNICIAN)]
    [HttpGet("inactive")]
    [SwaggerResponse(200, "Get Inactive Tasks", typeof(List<GetTicketTaskResponse>))]
    public async Task<IActionResult> GetInActiveTasks([FromQuery] int? ticketId)
    {
        try
        {
            var result = await _ticketTaskService.GetInActiveTasks(CurrentUserID, ticketId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Get Ticket Task by Id", typeof(GetTicketTaskResponse))]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _ticketTaskService.GetById(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPost("new")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTicketTaskRequest model)
    {
        try
        {
            await _ticketTaskService.Create(model, CurrentUserID);
            var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == model.TicketId);
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has got new task: {model.Title}", CurrentUserID);
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
            await _messagingService.SendNotification("ITSDS", $"Task [{req.Title}] has been updated", CurrentUserID);
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
            var task = await _ticketTaskService.GetById(taskId);
            await _messagingService.SendNotification("ITSDS", $"Task [{task.Title}] has been removed", CurrentUserID);
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
            var statusName = DataResponse.GetEnumDescription(newStatus);
            await _messagingService.SendNotification("ITSDS", $"Status has been updated to [{statusName}]", CurrentUserID);
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
