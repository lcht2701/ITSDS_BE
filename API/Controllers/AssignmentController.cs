using API.DTOs.Requests.Tickets;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;
using Persistence.Services.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/ticket/assign")]

public class AssignmentController : BaseController
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Team> _teamRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IAssignmentService _assignmentService;

    public AssignmentController(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Team> teamRepository, IRepositoryBase<Assignment> assignmentRepository, IRepositoryBase<User> userRepository, IAssignmentService assignmentService)
    {
        _ticketRepository = ticketRepository;
        _teamRepository = teamRepository;
        _assignmentRepository = assignmentRepository;
        _userRepository = userRepository;
        _assignmentService = assignmentService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAssignments()
    {
        var entity = await _assignmentRepository.ToListAsync();
        return Ok(entity);
    }

    [Authorize]
    [HttpGet("technician/{technicianId}")]
    public async Task<IActionResult> GetAssignmentsByTechnician(int technicianId)
    {
        var entity = await _assignmentRepository.WhereAsync(x => x.TechnicianId.Equals(technicianId));
        return Ok(entity);
    }

    [Authorize]
    [HttpGet("{assignmentId}/technician-info")]
    public async Task<IActionResult> GetTechnicianOfAssignment(int assignmentId, int technicianId)
    {
        var check  = await _assignmentRepository.FirstOrDefaultAsync(x => x.TechnicianId.Equals(technicianId) && x.Id.Equals(assignmentId));
        if (check == null)
        {
            throw new NotFoundException("Technician is not assigned with this assignment.");
        }
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(check.TechnicianId));
        return Ok(user);
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignmentById(int id)
    {
        var entity = await _assignmentRepository.FoundOrThrow(x => x.Id.Equals(id), new NotFoundException("No technician assigned to this ticket"));
        return Ok(entity);
    }

    [Authorize]
    [HttpPatch("{ticketId}/assign-team")]
    public async Task<IActionResult> AssignTeamToTicketManual(int ticketId, int teamId)
    {
        Ticket ticket = await _ticketRepository.FoundOrThrow(o => o.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        var team = await _teamRepository.FoundOrThrow(o => o.Id.Equals(teamId), new NotFoundException("Team not found"));
        ticket.TeamId = teamId;
        return Ok("Assign team succesfully");
    }

    [Authorize]
    [HttpPatch("{ticketId}/assign-technician")]
    public async Task<IActionResult> AssignTechnicianToTicketManual(int ticketId, int technicianId)
    {
        try
        {
            // Get the ticket
            Ticket ticket = await _ticketRepository.FoundOrThrow(
                o => o.Id.Equals(ticketId),
                new NotFoundException("Ticket not found")
            );

            bool success = await _assignmentService.AssignTechnicianToTicket(ticketId, technicianId);
            if (!success)
            {
                return BadRequest("Technician assignment failed.");
            }

            return Ok("Technician assigned successfully.");
        }
        catch (NotFoundException)
        {
            return NotFound("Ticket not found.");
        }
        catch (Exception)
        {
            // Log the exception for debugging purposes
            // You might also want to return a more informative error message
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    [Authorize]
    [HttpPatch("{ticketId}/update-technician")]
    public async Task<IActionResult> UpdateTechnicianAssignedToTicket(int ticketId, int newTechnicianId)
    {
        try
        {
            // Get the ticket
            Ticket ticket = await _ticketRepository.FoundOrThrow(
                o => o.Id.Equals(ticketId),
                new NotFoundException("Ticket not found")
            );

            bool success;

            // Update the technician assigned to the ticket without a team using the service
            success = await _assignmentService.UpdateTechnicianAssignment(ticketId, newTechnicianId);

            if (success)
            {
                return Ok("Technician assignment updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update technician assignment.");
            }
        }
        catch (NotFoundException)
        {
            return NotFound("Ticket not found.");
        }
        catch (Exception)
        {
            // Log the exception for debugging purposes
            // You might also want to return a more informative error message
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveAssignment(int id)
    {
        var entity = await _assignmentRepository.FoundOrThrow(x => x.Id.Equals(id), new NotFoundException("Assignment not found."));
        await _assignmentRepository.DeleteAsync(entity);
        return Ok("Remove successfully.");
    }

}


