using API.DTOs.Requests.Assignments;
using API.Services.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/assign")]

public class AssignmentController : BaseController
{
    private readonly IAssignmentService _assignmentService;
    private readonly IMessagingService _messagingService;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Ticket> _ticketRepository;

    public AssignmentController(IAssignmentService assignmentService, IMessagingService messagingService, IRepositoryBase<User> userRepository, IRepositoryBase<Ticket> ticketRepository)
    {
        _assignmentService = assignmentService;
        _messagingService = messagingService;
        _userRepository = userRepository;
        _ticketRepository = ticketRepository;
    }

    [Authorize]
    [HttpGet("get-technicians")]
    public async Task<IActionResult> GetListOfTechnician(int? teamId)
    {
        try
        {
            var result = await _assignmentService.GetListOfTechnician(teamId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAssignments()
    {
        try
        {
            var result = await _assignmentService.Get();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("technician/{technicianId}")]
    public async Task<IActionResult> GetAssignmentsByTechnician(int technicianId)
    {
        try
        {
            var result = await _assignmentService.GetByTechnician(technicianId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("team/{teamId}")]
    public async Task<IActionResult> GetAssignmentsByTeam(int teamId)
    {
        try
        {
            var result = await _assignmentService.GetByTeam(teamId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignmentById(int id)
    {
        try
        {
            var result = await _assignmentService.GetById(id);
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
    [HttpPost("{ticketId}/assign")]
    public async Task<IActionResult> AssignTicket(int ticketId, [FromBody] AssignTicketManualRequest req)
    {
        try
        {
            await _assignmentService.Assign(ticketId, req);
            #region Notification
            var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId);
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been assigned",
                    CurrentUserID);
            if (req.TechnicianId != null)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been assigned to you",
                    (int)req.TechnicianId);
            }
            if (ticket.RequesterId != null)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been assigned!",
                    (int)ticket.RequesterId);
            }

            #endregion
            return Ok("Assigned Ticket Successfully");
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


    [Authorize]
    [HttpPatch("{ticketId}")]
    public async Task<IActionResult> UpdateTicketAssignment(int ticketId, [FromBody] UpdateTicketAssignmentManualRequest req)
    {
        try
        {
            await _assignmentService.Update(ticketId, req);
            #region Notification
            var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId);
            await _messagingService.SendNotification("ITSDS", $"Ticket Assignment for [{ticket.Title}] has been updated",
                    CurrentUserID);
            if (req.TechnicianId != null)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been assigned to you",
                    (int)req.TechnicianId);
            }
            if (ticket.RequesterId != null)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket Assignment for [{ticket.Title}] has been updated",
                    (int)ticket.RequesterId);
            }

            #endregion
            return Ok("Updated Successfully");
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


    [Authorize]
    [HttpDelete("{ticketId}")]
    public async Task<IActionResult> RemoveAssignmentByTicketId(int ticketId)
    {
        try
        {
            await _assignmentService.Remove(ticketId);
            #region Notification
            var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId);
            await _messagingService.SendNotification("ITSDS", $"Ticket Assignment for [{ticket.Title}] has been removed",
                    CurrentUserID);
            if (ticket.RequesterId != null)
            {
                await _messagingService.SendNotification("ITSDS", $"Ticket Assignment for [{ticket.Title}] has been removed",
                    (int)ticket.RequesterId);
            }
            #endregion
            return Ok("Removed Successfully.");
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
}


