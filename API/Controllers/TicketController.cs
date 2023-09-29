using API.DTOs.Tickets.Requests;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories;

namespace API.Controllers;

[Route("/v1/itsds/ticket")]

public class TicketController : BaseController
{
    private readonly RepositoryBase<Ticket> _ticketRepository;
    private readonly RepositoryBase<User> _userRepository;
    private readonly RepositoryBase<Team> _teamRepository;

    public TicketController(RepositoryBase<Ticket> ticketRepository, RepositoryBase<User> userRepository, RepositoryBase<Team> teamRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var result = await _ticketRepository.ToListAsync();
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetTicketById(int ticketId)
    {
        var result = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        return Ok(result);
    }

    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTicketsByRequester(int requesterId)
    {
        var result = await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(requesterId));
        if (result.Count == 0)
        {
            throw new NotFoundException("No tickets was found for this user");
        }
        return Ok(result);
    }

    [Authorize]
    [HttpGet("team/{teamId}")]
    public async Task<IActionResult> GetTicketsResponsibleByTeam(int teamId)
    {
        var result = await _ticketRepository.WhereAsync(x => x.Assignment.TeamId.Equals(teamId));
        if (result.Count == 0)
        {
            throw new NotFoundException("No tickets was found for this team");
        }
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{ticketId}")]
    public async Task<IActionResult> UpdateTicketInformation(int ticketId, [FromBody]UpdateTicketRequest model)
    {
        var target = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        var entity = Mapper.Map(model, new Ticket());
        await _ticketRepository.UpdateAsync(entity);
        return Accepted(entity);
    }

    [Authorize]
    [HttpDelete("{ticketId}")]
    public async Task<IActionResult> DeleteTicket(int ticketId, [FromBody] UpdateTicketRequest model)
    {
        var target = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        await _ticketRepository.DeleteAsync(target);
        return Accepted(target);
    }

}


