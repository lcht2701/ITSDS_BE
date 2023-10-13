using API.DTOs.Requests.Assignments;
using API.DTOs.Requests.Tickets;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509;
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
        return entity != null ? Ok(entity) : Ok("No assignments.");
    }

    [Authorize]
    [HttpGet("technician/{technicianId}")]
    public async Task<IActionResult> GetAssignmentsByTechnician(int technicianId)
    {
        var entity = await _assignmentRepository.WhereAsync(x => x.TechnicianId.Equals(technicianId), new string[] { "Ticket", "Team", "Technician" });
        return entity != null ? Ok(entity) : Ok("No assignments of this technician.");
    }

    [Authorize]
    [HttpGet("team/{teamId}")]
    public async Task<IActionResult> GetAssignmentsByTeam(int teamId)
    {
        var entity = await _assignmentRepository.WhereAsync(x => x.TeamId.Equals(teamId), new string[] { "Ticket", "Team", "Technician" });
        return entity != null ? Ok(entity) : Ok("No assignments of this team.");
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignmentById(int id)
    {
        var entity = await _assignmentRepository.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Ticket", "Team", "Technician" });
        return entity != null ? Ok(entity) : throw new BadRequestException("Assignment is not found");
    }

    [Authorize]
    [HttpPatch("{ticketId}/assign")]
    public async Task<IActionResult> AssignTicketManual(int ticketId, [FromBody] AssignTicketManualRequest req)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(ticketId));
        if (req.TeamId != null && req.TechnicianId != null)
        {
            var isMemberOfTeam = await IsTechnicianMemberOfTeamAsync(req.TechnicianId, req.TeamId);
            if (!isMemberOfTeam)
            {
                throw new BadRequestException("This technician is not in this team");
            }
            else
            {
                var entity = Mapper.Map(req, new Assignment());
                await _assignmentRepository.CreateAsync(entity);
                ticket.AssignmentId = entity.Id;
            }
        }

        return Ok("Assign Successfully");
    }

    [Authorize]
    [HttpPatch("{ticketId}/update")]
    public async Task<IActionResult> UpdateTicketAssignmentManual(int ticketId, [FromBody] UpdateTicketAssignmentManualRequest req)
    {
        var ticket = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new BadRequestException("Ticket Not Found"));
        var target = await _assignmentRepository.FoundOrThrow(x => x.Id.Equals(ticket.AssignmentId), new BadRequestException("Ticket has not been assigned"));

        if (req.TeamId != null && req.TechnicianId != null)
        {
            var isMemberOfTeam = await IsTechnicianMemberOfTeamAsync(req.TechnicianId, req.TeamId);
            if (!isMemberOfTeam)
            {
                throw new BadRequestException("This technician is not in this team");
            }
            else
            {
                var entity = Mapper.Map(req, target);
                await _assignmentRepository.UpdateAsync(entity);
            }
        }

        return Ok("Update Successfully");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveAssignment(int ticketId)
    {
        var ticket = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new BadRequestException("Ticket Not Found"));
        var entity = await _assignmentRepository.FoundOrThrow(x => x.Id.Equals(ticket.AssignmentId), new BadRequestException("Ticket has not been assigned"));
        await _assignmentRepository.DeleteAsync(entity);
        return Ok("Remove successfully.");
    }

    private async Task<bool> IsTechnicianMemberOfTeamAsync(int? technicianId, int? teamId)
    {
        var check = await _teamMemberRepository.FirstOrDefaultAsync(x =>
            x.MemberId.Equals(technicianId) && x.TeamId.Equals(teamId));

        return check != null;
    }

}


