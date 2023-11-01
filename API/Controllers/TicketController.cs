using API.DTOs.Requests.Tickets;
using API.Services.Implements;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models.Tickets;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/ticket")]
public class TicketController : BaseController
{
    private readonly IAuditLogService _auditLogService;
    private readonly ITicketService _ticketService;
    private readonly IMessagingService _messagingService;
    private readonly IRepositoryBase<Ticket> _ticketRepository;

    public TicketController(IAuditLogService auditLogService, ITicketService ticketService,
        IMessagingService messagingService, IRepositoryBase<Ticket> ticketRepository)
    {
        _auditLogService = auditLogService;
        _ticketService = ticketService;
        _messagingService = messagingService;
        _ticketRepository = ticketRepository;
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllTicket()
    {
        var result = await _ticketService.Get();
        return Ok(result);
    }

    [Authorize]
    [HttpGet("ticket-status")]
    public async Task<IActionResult> GetStatuses()
    {
        var result = await _ticketService.GetTicketStatuses();
        return Ok(result);
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
    [HttpGet("assign/available")]
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

    [Authorize(Roles = Roles.TECHNICIAN)]
    [HttpGet("assign/done")]
    public async Task<IActionResult> GetCompleteAssignedTickets()
    {
        try
        {
            var response = await _ticketService.GetCompletedAssignedTickets(CurrentUserID);
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

            //Chỉnh lại tgian hẹn giờ sau
            string jobId = BackgroundJob.Schedule(
                () => _ticketService.AssignSupportJob(entity.Id),
                TimeSpan.FromMinutes(10));
            RecurringJob.AddOrUpdate(
                jobId + "_Cancellation",
                () => _ticketService.CancelAssignSupportJob(jobId, entity.Id),
                "*/5 * * * * *"); //Every 5 secs
            await _messagingService.SendNotification($"Ticket [{model.Title}] has been created", CurrentUserID);
            return Ok("Ticket created and scheduled for assignment.");
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
            Ticket? original = (Ticket?)await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            var updated = await _ticketService.UpdateByCustomer(ticketId, model);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            await _messagingService.SendNotification($"Ticket [{model.Title}] has been updated", CurrentUserID);
            return Ok("Update Successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
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
            if (await _ticketService.IsTicketAssigned(entity.Id))
            {
                return Ok("Created Successfully");
            }
            else
            {
                //Chỉnh lại tgian hẹn giờ sau
                string jobId = BackgroundJob.Schedule(
                    () => _ticketService.AssignSupportJob(entity.Id),
                    TimeSpan.FromMinutes(10));
                RecurringJob.AddOrUpdate(
                    jobId + "_Cancellation",
                    () => _ticketService.CancelAssignSupportJob(jobId, entity.Id),
                    "*/5 * * * * *"); //Every 5
                await _messagingService.SendNotification($"Ticket [{model.Title}] has been created", CurrentUserID);
                if (model.RequesterId != null)
                {
                    await _messagingService.SendNotification($"Ticket [{model.Title}] has been created",
                        (int)model.RequesterId);
                }

                return Ok("Ticket created and scheduled for assignment.");
            }
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
            Ticket? original = (Ticket?)await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            var updated = await _ticketService.UpdateByManager(ticketId, model);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            await _messagingService.SendNotification($"Ticket [{model.Title}] has been updated", CurrentUserID);
            if (model.RequesterId != null)
            {
                await _messagingService.SendNotification($"Ticket [{model.Title}] has been updated",
                    (int)model.RequesterId);
            }

            return Ok("Update Successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
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
            var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId);
            await _messagingService.SendNotification($"Ticket [{ticket.Title}] has been removed", CurrentUserID);
            return Ok("Removed Successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPatch("modify-status")]
    public async Task<IActionResult> ModifyTicketStatus(int ticketId, TicketStatus newStatus)
    {
        try
        {
            var original = await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            await _ticketService.ModifyTicketStatus(ticketId, newStatus);
            var updated = await _ticketService.GetById(ticketId);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            //Send Notification
            await _messagingService.SendNotification($"Status of ticket [{updated.Title}] has been update",
                CurrentUserID);
            if (updated.RequesterId != null)
            {
                await _messagingService.SendNotification($"Status of ticket [{updated.Title}] has been update",
                    (int)updated.RequesterId);
            }

            return Ok("Status Updated Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Ticket is not exist");
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.CUSTOMER}")]
    [HttpPatch("cancel")]
    public async Task<IActionResult> CancelTicket(int ticketId)
    {
        try
        {
            var original = await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            await _ticketService.CancelTicket(ticketId, CurrentUserID);
            var updated = await _ticketService.GetById(ticketId);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            await _messagingService.SendNotification($"Ticket [{updated.Title}] has been cancelled", CurrentUserID);
            return Ok("Ticket Cancelled Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Ticket is not exist");
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.CUSTOMER}")]
    [HttpPatch("close")]
    public async Task<IActionResult> CloseTicket(int ticketId)
    {
        try
        {
            var original = await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            await _ticketService.CloseTicket(ticketId, CurrentUserID);
            var updated = await _ticketService.GetById(ticketId);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            await _messagingService.SendNotification($"Ticket [{updated.Title}] has been closed", CurrentUserID);
            return Ok("Ticket Closed Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Ticket is not exist");
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}