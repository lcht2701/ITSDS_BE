using API.DTOs.Requests.Assignments;
using API.DTOs.Requests.Tickets;
using Domain.Constants.Cases;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;
using Persistence.Services.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/assign")]

public class AssignmentController : BaseController
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IStatusTrackingService _statusTrackingService;
    private readonly IAssignmentService _assignmentService;

    public AssignmentController(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IRepositoryBase<TeamMember> teamMemberRepository, IStatusTrackingService statusTrackingService, IAssignmentService assignmentService)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _teamMemberRepository = teamMemberRepository;
        _statusTrackingService = statusTrackingService;
        _assignmentService = assignmentService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAssignments()
    {
        var entity = await _assignmentRepository.GetAsync(navigationProperties: new string[] { "Team", "Technician" });
        return entity != null ? Ok(entity) : Ok("No assignments.");
    }

    [Authorize]
    [HttpGet("technician/{technicianId}")]
    public async Task<IActionResult> GetAssignmentsByTechnician(int technicianId)
    {
        var entity = await _assignmentRepository.WhereAsync(x => x.TechnicianId.Equals(technicianId), new string[] { "Team", "Technician" });
        return entity != null ? Ok(entity) : Ok("No assignments of this technician.");
    }

    [Authorize]
    [HttpGet("team/{teamId}")]
    public async Task<IActionResult> GetAssignmentsByTeam(int teamId)
    {
        var entity = await _assignmentRepository.WhereAsync(x => x.TeamId.Equals(teamId), new string[] { "Team", "Technician" });
        return entity != null ? Ok(entity) : Ok("No assignments of this team.");
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignmentById(int id)
    {
        var entity = await _assignmentRepository.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Team", "Technician" });
        return entity != null ? Ok(entity) : throw new BadRequestException("Assignment is not found");
    }

    [Authorize]
    [HttpPost("{ticketId}/assign")]
    public async Task<IActionResult> AssignTicket(int ticketId, [FromBody] AssignTicketManualRequest req)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId);
        if (ticket == null)
        {
            return NotFound("Ticket not found.");
        }

        var existingAssignment = await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId == ticketId);

        if (existingAssignment != null)
        {
            return BadRequest("This ticket is already assigned.");
        }

        switch (GetAssignmentCase(req.TechnicianId, req.TeamId))
        {
            case AssignmentCase.NullNull:
                return Ok("Both TeamId and TechnicianId must be specified.");
            case AssignmentCase.NotNullNull:
                var assignment = new Assignment()
                {
                    TicketId = ticketId,
                    TechnicianId = req.TechnicianId
                };
                await _assignmentRepository.CreateAsync(assignment);
                await _statusTrackingService.UpdateTicketStatusTo(ticket, TicketStatus.Assigned);
                return Ok("Assigned successfully");
            case AssignmentCase.NullNotNull:
                req.TechnicianId = await _assignmentService.FindTechnicianWithLeastAssignments(req.TeamId);

                assignment = new Assignment()
                {
                    TicketId = ticketId,
                    TeamId = req.TeamId,
                    TechnicianId = req.TechnicianId
                };

                await _assignmentRepository.CreateAsync(assignment);
                await _statusTrackingService.UpdateTicketStatusTo(ticket, TicketStatus.Assigned);
                return Ok("Assign Successfully");
            case AssignmentCase.NotNullNotNull:

                if (await IsTechnicianMemberOfTeamAsync(req.TechnicianId, req.TeamId) == null)
                {
                    return BadRequest("This technician is not a member of the specified team.");
                }

                assignment = new Assignment()
                {
                    TicketId = ticketId,
                    TeamId = req.TeamId,
                    TechnicianId = req.TechnicianId
                };

                await _assignmentRepository.CreateAsync(assignment);
                await _statusTrackingService.UpdateTicketStatusTo(ticket, TicketStatus.Assigned);
                return Ok("Assigned successfully");
        }
        return BadRequest("Invalid request.");
    }

    //Cần update lại
    [Authorize]
    [HttpPatch("{ticketId}/update")]
    public async Task<IActionResult> UpdateTicketAssignmentManual(int ticketId, [FromBody] UpdateTicketAssignmentManualRequest req)
    {
        var ticket = await _ticketRepository.FoundOrThrow(x => x.Id == ticketId, new BadRequestException("Assignment Not Found"));
        var target = await _assignmentRepository.FoundOrThrow(x => x.TicketId.Equals(ticket.Id), new BadRequestException("Assignment Not Found"));

        switch (GetAssignmentCase(req.TechnicianId, req.TeamId))
        {
            case AssignmentCase.NullNull:
                await _assignmentRepository.DeleteAsync(target);
                return Ok("Update Successfully");

            case AssignmentCase.NotNullNull:
                var entity = Mapper.Map(req, target);
                entity.TeamId = null;
                await _assignmentRepository.UpdateAsync(entity);
                return Ok("Update successfully");

            case AssignmentCase.NullNotNull:
                req.TechnicianId = await _assignmentService.FindTechnicianWithLeastAssignments(req.TeamId);

                entity = Mapper.Map(req, target);
                entity.TechnicianId = null;

                await _assignmentRepository.UpdateAsync(entity);
                return Ok("Update Successfully");

            case AssignmentCase.NotNullNotNull:
                if (await IsTechnicianMemberOfTeamAsync(req.TechnicianId, req.TeamId) == null)
                {
                    return BadRequest("This technician is not a member of the specified team.");
                }

                entity = Mapper.Map(req, target);
                await _assignmentRepository.UpdateAsync(entity);
                return Ok("Update Successfully");

            default:
                return BadRequest("Invalid request.");
        }
    }

    [Authorize]
    [HttpDelete("{ticketId}")]
    public async Task<IActionResult> RemoveAssignmentByTicketId(int ticketId)
    {
        var entity = await _assignmentRepository.FoundOrThrow(x => x.TicketId.Equals(ticketId), new BadRequestException("Ticket has not been assigned"));
        await _assignmentRepository.DeleteAsync(entity);
        // Cần kiểm tra lại logic
        //await _statusTrackingService.UpdateTicketStatusTo(ticket, TicketStatus.Open);
        return Ok("Remove successfully.");
    }

    private async Task<TeamMember> IsTechnicianMemberOfTeamAsync(int? technicianId, int? teamId)
    {
        var check = await _teamMemberRepository.FirstOrDefaultAsync(x =>
            x.MemberId.Equals(technicianId) && x.TeamId.Equals(teamId));

        return check;
    }

    private AssignmentCase GetAssignmentCase(int? technicianId, int? teamId)
    {
        if (technicianId == null && teamId == null)
        {
            return AssignmentCase.NullNull;
        }
        else if (technicianId != null && teamId == null)
        {
            return AssignmentCase.NotNullNull;
        }
        else if (technicianId == null && teamId != null)
        {
            return AssignmentCase.NullNotNull;
        }
        else
        {
            return AssignmentCase.NotNullNotNull;
        }
    }
}


