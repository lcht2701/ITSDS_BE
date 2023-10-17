﻿using Domain.Constants.Enums;
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

    public AssignmentController(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IRepositoryBase<TeamMember> teamMemberRepository, IStatusTrackingService statusTrackingService)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _teamMemberRepository = teamMemberRepository;
        _statusTrackingService = statusTrackingService;
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
    public async Task<IActionResult> AssignTicketManual(int ticketId, int? teamId, int? technicianId)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId);

        if (ticket == null)
        {
            return NotFound("Ticket not found");
        }

        var existingAssignment = await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId == ticketId);

        if (existingAssignment != null)
        {
            return BadRequest("Ticket is already assigned");
        }

        if (teamId == null || technicianId == null)
        {
            return BadRequest("Both teamId and technicianId must be provided to make the assignment.");
        }

        if (!await IsTechnicianMemberOfTeamAsync(technicianId.Value, teamId.Value))
        {
            return BadRequest("This technician is not in this team");
        }

        var assignment = new Assignment
        {
            TicketId = ticketId,
            TeamId = teamId.Value,
            TechnicianId = technicianId.Value
        };

        await _assignmentRepository.CreateAsync(assignment);
        await _statusTrackingService.UpdateTicketStatusTo(ticket, TicketStatus.Assigned);

        return Ok("Assign Successfully");
    }



    [Authorize]
    [HttpPatch("{ticketId}/update")]
    public async Task<IActionResult> UpdateTicketAssignmentManual(int ticketId, int? teamId, int? technicianId)
    {
        var target = await _assignmentRepository.FoundOrThrow(x => x.TicketId.Equals(ticketId), new BadRequestException("Ticket Not Found"));

        if (teamId == null || technicianId == null)
        {
            return BadRequest("Both teamId and technicianId must be provided to update the assignment.");
        }

        if (!await IsTechnicianMemberOfTeamAsync(technicianId.Value, teamId.Value))
        {
            return BadRequest("This technician is not in this team");
        }

        var req = new Assignment()
        {
            TeamId = teamId.Value,
            TechnicianId = technicianId.Value
        };

        var entity = Mapper.Map(req, target);
        await _assignmentRepository.UpdateAsync(entity);

        return Ok("Update Successfully");
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

    private async Task<bool> IsTechnicianMemberOfTeamAsync(int? technicianId, int? teamId)
    {
        var check = await _teamMemberRepository.FirstOrDefaultAsync(x =>
            x.MemberId.Equals(technicianId) && x.TeamId.Equals(teamId));

        return check is not null;
    }

}


