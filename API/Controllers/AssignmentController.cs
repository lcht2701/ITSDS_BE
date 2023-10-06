using API.DTOs.Requests.Tickets;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/ticket/assign")]

public class AssignmentController : BaseController
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Team> _teamRepository;

    public AssignmentController(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<User> userRepository, IRepositoryBase<Team> teamRepository, IRepositoryBase<Assignment> assignmentRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _assignmentRepository = assignmentRepository;
    }

    [Authorize]
    [HttpPost("assign-team")]
    public async Task<IActionResult> AssignTicketToTeamManual(int ticketId, int teamId)
    {
        Ticket ticket = await _ticketRepository.FoundOrThrow(o => o.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        if (ticket.TicketStatus == TicketStatus.Open)
        {
            return BadRequest("Ticket has already executed");
        }
        var team = _teamRepository.FoundOrThrow(o => o.Id.Equals(teamId), new NotFoundException("Team not found"));
        
        return Ok();
    }

    [Authorize]
    [HttpPost("assign-technician")]
    public async Task<IActionResult> AssignTicketToTechnicianManual(int ticketId, int teamId)
    {
        return Ok();
    }
}


