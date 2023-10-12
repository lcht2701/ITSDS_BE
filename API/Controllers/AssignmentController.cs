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
        var entity = await _assignmentRepository.GetAsync(navigationProperties: new string[] { "Ticket", "Team", "Technician" });
        return Ok(entity);
    }

    [Authorize]
    [HttpGet("technician/{technicianId}")]
    public async Task<IActionResult> GetAssignmentsByTechnician(int technicianId)
    {
        var entity = await _assignmentRepository.WhereAsync(x => x.TechnicianId.Equals(technicianId), new string[] { "Ticket", "Team", "Technician" });
        return Ok(entity);
    }

    [Authorize]
    [HttpGet("team/{teamId}")]
    public async Task<IActionResult> GetAssignmentsByTeam(int teamId)
    {
        var entity = await _assignmentRepository.WhereAsync(x => x.TeamId.Equals(teamId), new string[] { "Ticket", "Team", "Technician" });
        return Ok(entity);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignmentById(int id)
    {
        var entity = await _assignmentRepository.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Ticket", "Team", "Technician" });
        if (entity == null)
        {
            return Ok("Assignment not found.");
        }
        return Ok(entity);
    }

    [Authorize]
    [HttpPatch("{ticketId}/assign")]
    public async Task<IActionResult> AssignTicketManual([FromBody] AssignTicketManualRequest req)
    {
        if (req.TeamId != null && req.TechnicianId != null)
        {
            var isMemberOfTeam = await _teamMemberRepository.FirstOrDefaultAsync(x =>
                    x.MemberId.Equals(req.TechnicianId) &&
                    x.TeamId.Equals(req.TeamId)) ?? throw new BadRequestException("This technician is not in this team");
        }

        var entity = Mapper.Map(req, new Assignment());
        await _assignmentRepository.CreateAsync(entity);
        return Ok("Assign Successfully");
    }

    [Authorize]
    [HttpPatch("{ticketId}/update")]
    public async Task<IActionResult> UpdateTicketAssignmentManual(int assignmentId, [FromBody] UpdateTicketAssignmentManualRequest req)
    {
        var target = await _assignmentRepository.FirstOrDefaultAsync(x => x.Id.Equals(assignmentId));
        if (target == null)
        {
            throw new BadRequestException("Ticket Not Found");
        }
        else
        {
            if (req.TeamId != null && req.TechnicianId != null)
            {
                var isMemberOfTeam = await _teamMemberRepository.FirstOrDefaultAsync(x =>
                        x.MemberId.Equals(target.TechnicianId) &&
                        x.TeamId.Equals(target.TeamId)) ?? throw new BadRequestException("This technician is not in this team");
            }
        }
        var entity = Mapper.Map(req, target);
        return Ok("Update Successfully");
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


