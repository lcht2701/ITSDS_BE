using API.DTOs.Requests.Assignments;
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
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;

    public AssignmentController(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IRepositoryBase<TeamMember> teamMemberRepository)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _teamMemberRepository = teamMemberRepository;
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
    [HttpGet("team/{teamId}")]
    public async Task<IActionResult> GetAssignmentsByTeam(int teamId)
    {
        var entity = await _assignmentRepository.WhereAsync(x => x.TeamId.Equals(teamId));
        return Ok(entity);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignmentById(int id)
    {
        var entity = await _assignmentRepository.FoundOrThrow(x => x.Id.Equals(id), new NotFoundException("No technician assigned to this ticket"));
        return Ok(entity);
    }

    [Authorize]
    [HttpPatch("{ticketId}/assign")]
    public async Task<IActionResult> AssignTicketManual([FromBody] AssignTicketManualRequest model)
    {
        var ticket = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(model.TicketId), new NotFoundException("Ticket not found"));
        if (model.TechnicianId != null)
        {
            var isMemberOfTeam = await _teamMemberRepository.FirstOrDefaultAsync(x =>
                    x.MemberId.Equals(model.TechnicianId) &&
                    x.TeamId.Equals(model.TeamId)) ?? throw new BadRequestException("This technician is not in this team");
        }

        var entity = Mapper.Map(model, new Assignment());
        await _assignmentRepository.CreateAsync(entity);
        return Ok("Assign Successfully");
    }

    [Authorize]
    [HttpPatch("{ticketId}/update")]
    public async Task<IActionResult> UpdateTicketAssignmentManual(int assignmentId, [FromBody] UpdateTicketAssignmentManualRequest req)
    {
        var target = await _assignmentRepository.FoundOrThrow(x => x.Id.Equals(assignmentId), new NotFoundException("Assignment not found"));
        if (target.TechnicianId != null)
        {
            var isMemberOfTeam = await _teamMemberRepository.FirstOrDefaultAsync(x =>
                    x.MemberId.Equals(target.TechnicianId) &&
                    x.TeamId.Equals(target.TeamId)) ?? throw new BadRequestException("This technician is not in this team");
        }
        var entity = Mapper.Map(req, target);
        return Ok();
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


