using API.DTOs.Requests.Tickets;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
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
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;

    public TicketController(IAuditLogService auditLogService, ITicketService ticketService, IMessagingService messagingService, IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<User> userRepository, IRepositoryBase<Assignment> assignmentRepository)
    {
        _auditLogService = auditLogService;
        _ticketService = ticketService;
        _messagingService = messagingService;
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _assignmentRepository = assignmentRepository;
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
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
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
            Ticket entity = await _ticketService.CreateByCustomer(CurrentUserID, model);
            await _auditLogService.TrackCreated(entity.Id, Tables.TICKET, CurrentUserID);
            #region Notification
            await _messagingService.SendNotification("ITSDS", $"Ticket [{model.Title}] has been created and scheduled for assignment", CurrentUserID);
            foreach (var managerId in await GetManagerIdsList())
            {
                await _messagingService.SendNotification("ITSDS", $"New ticket [{model.Title}] has been created", managerId);
            }
            #endregion

            #region Background Job for auto assign
            string jobId = BackgroundJob.Schedule(
                        () => _ticketService.AssignSupportJob(entity.Id),
                        TimeSpan.FromMinutes(5));
            BackgroundJob.ContinueJobWith(
                jobId, () => SendNotificationAfterAssignment(entity));
            RecurringJob.AddOrUpdate(
                jobId + "_Cancellation",
                () => _ticketService.CancelAssignSupportJob(jobId, entity.Id),
                "*/5 * * * * *"); //Every 5 secs
            #endregion
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
            #region Notification
            var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(CurrentUserID));
            await _messagingService.SendNotification("ITSDS", $"Ticket [{model.Title}] has been updated", CurrentUserID);
            foreach (var managerId in await GetManagerIdsList())
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{model.Title}] has been updated by [{currentUser.Username}]", managerId);
            }
            #endregion
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
            #region Notification
            foreach (var managerId in await GetManagerIdsList())
            {
                await _messagingService.SendNotification("ITSDS", $"Status of ticket [{model.Title}] has been updated", managerId);

            }
            if (model.RequesterId != null)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{model.Title}] has been created",
                    (int)model.RequesterId);
            }
            #endregion
            if (await _ticketService.IsTicketAssigned(entity.Id))
            {
                return Ok("Created Successfully");
            }
            else
            {
                #region Background Job for auto assign
                string jobId = BackgroundJob.Schedule(
                            () => _ticketService.AssignSupportJob(entity.Id),
                            TimeSpan.FromMinutes(5));
                //Send Notification After The Ticket Is Assigned By BackgroundJob
                BackgroundJob.ContinueJobWith(
                jobId, () => SendNotificationAfterAssignment(entity));

                RecurringJob.AddOrUpdate(
                    jobId + "_Cancellation",
                    () => _ticketService.CancelAssignSupportJob(jobId, entity.Id),
                    "*/5 * * * * *"); //Every 5 secs
                #endregion
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
            #region Notification
            await _messagingService.SendNotification("ITSDS", $"Ticket [{model.Title}] has been updated", CurrentUserID);
            if (model.RequesterId != null)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{model.Title}] has been updated",
                    (int)model.RequesterId);
            }
            var technicianId = await GetTechnicianAssigned(ticketId);
            if (technicianId != 0)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{model.Title}] has been updated", technicianId);
            }
            foreach (var managerId in await GetManagerIdsList())
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{model.Title}] has been updated", managerId);
            }
            #endregion
            return Ok("Update Successfully");
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

    [Authorize(Roles = Roles.TECHNICIAN)]
    [HttpPatch("technician/{ticketId}")]
    public async Task<IActionResult> UpdateTicketByTechnician(int ticketId, [FromBody] TechnicianAddDetailRequest model)
    {
        try
        {
            Ticket? original = (Ticket?)await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            var updated = await _ticketService.UpdateByTechnician(ticketId, model);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            #region Notification
            await _messagingService.SendNotification("ITSDS", $"Ticket [{updated.Title}] has been updated", CurrentUserID);
            if (updated.RequesterId != null)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{updated.Title}] has been updated",
                    (int)updated.RequesterId);
            }
            var technicianId = await GetTechnicianAssigned(ticketId);
            if (technicianId != 0)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{updated.Title}] has been updated", technicianId);
            }
            foreach (var managerId in await GetManagerIdsList())
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{updated.Title}] has been updated", managerId);
            }
            #endregion
            return Ok("Update Successfully");
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

    [Authorize(Roles = Roles.MANAGER)]
    [HttpDelete("manager/{ticketId}")]
    public async Task<IActionResult> DeleteTicket(int ticketId)
    {
        try
        {
            var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId);
            #region Notification
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been removed", CurrentUserID);
            if (ticket.RequesterId != null)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been removed",
                    (int)ticket.RequesterId);
            }
            #endregion
            await _ticketService.Remove(ticketId);
            return Ok("Removed Successfully");
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
    [HttpPatch("modify-status")]
    public async Task<IActionResult> ModifyTicketStatus(int ticketId, TicketStatus newStatus)
    {
        try
        {
            Ticket? original = (Ticket?)await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            var updated = await _ticketService.ModifyTicketStatus(ticketId, newStatus);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            #region Notification
            if (updated.RequesterId != null)
            {
                await _messagingService.SendNotification("ITSDS", $"Status of ticket [{updated.Title}] has been updated to [{DataResponse.GetEnumDescription(updated.TicketStatus)}]",
                    (int)updated.RequesterId);
            }
            var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(CurrentUserID));
            switch (currentUser.Role)
            {
                case Role.Technician:
                    await _messagingService.SendNotification("ITSDS", $"Status of ticket [{updated.Title}] has been updated to [{DataResponse.GetEnumDescription(updated.TicketStatus)}]", CurrentUserID);
                    break;
                case Role.Manager:
                    var technicianId = await GetTechnicianAssigned(ticketId);
                    if (technicianId != 0)
                    {
                        await _messagingService.SendNotification("ITSDS", $"Status of ticket [{updated.Title}] has been updated to [ {DataResponse.GetEnumDescription(updated.TicketStatus)} ]", (int)technicianId);
                    }
                    break;
            }
            foreach (var managerId in await GetManagerIdsList())
            {
                await _messagingService.SendNotification("ITSDS", $"Status of ticket [{updated.Title}] has been updated to [ {DataResponse.GetEnumDescription(updated.TicketStatus)} ]", managerId);
            }

            #endregion
            return Ok("Status Updated Successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
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
            Ticket? original = (Ticket?)await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            var updated = await _ticketService.CancelTicket(ticketId, CurrentUserID);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            #region Notification
            var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(CurrentUserID));
            await _messagingService.SendNotification("ITSDS", $"Ticket [{updated.Title}] has been cancelled", CurrentUserID);
            foreach (var managerId in await GetManagerIdsList())
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{updated.Title}] has been cancelled", managerId);
            }
            #endregion
            return Ok("Ticket Cancelled Successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
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
            Ticket? original = (Ticket?)await _auditLogService.GetOriginalModel(ticketId, Tables.TICKET);
            var updated = await _ticketService.CloseTicket(ticketId, CurrentUserID);
            await _auditLogService.TrackUpdated(original, updated, CurrentUserID, ticketId, Tables.TICKET);
            #region Notification
            var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(CurrentUserID));
            await _messagingService.SendNotification("ITSDS", $"Ticket [{updated.Title}] has been closed", CurrentUserID);
            var technicianId = await GetTechnicianAssigned(ticketId);
            if (technicianId != 0) await _messagingService.SendNotification("ITSDS", $"Ticket [{updated.Title}] has been closed", technicianId);
            foreach (var managerId in await GetManagerIdsList())
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{updated.Title}] has been closed", managerId);
            }
            #endregion
            return Ok("Ticket Closed Successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
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

    private async Task<List<int>> GetManagerIdsList()
    {
        var managerIds = (await _userRepository.WhereAsync(x => x.Role == Role.Manager)).Select(x => x.Id).ToList();
        return managerIds;
    }
    private async Task<int> GetTechnicianAssigned(int ticketId)
    {
        var assignment = await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId == ticketId);
        if (assignment.TechnicianId != null)
        {
            return (int)assignment.TechnicianId;
        }
        else
        {
            return 0;
        }
    }

    [NonAction]
    public async Task SendNotificationAfterAssignment(Ticket ticket)
    {
        var techinicianId = (await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId == ticket.Id)).TechnicianId;
        if (techinicianId != null)
        {
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been assigned to you",
                (int)techinicianId);
        }
        if (ticket.RequesterId != null)
        {
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been assigned",
                (int)ticket.RequesterId);
        }
        foreach (var managerId in await GetManagerIdsList())
        {
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been assigned",
                managerId);
        }
    }
}