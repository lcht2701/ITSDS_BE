using API.DTOs.Requests.Assignments;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("/v1/itsds/assign")]

public class AssignmentController : BaseController
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
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
        catch (KeyNotFoundException)
        {
            return NotFound("Assignment is not exist");
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
            var result = await _assignmentService.Assign(ticketId, req);
            return result;
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
            var result = await _assignmentService.Update(ticketId, req);
            return result;
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
            return Ok("Removed Successfully.");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Assignment is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}


