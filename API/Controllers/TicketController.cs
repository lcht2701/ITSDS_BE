using API.DTOs.Requests.Tickets;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/ticket")]

public class TicketController : BaseController
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Team> _teamRepository;

    public TicketController(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<User> userRepository, IRepositoryBase<Team> teamRepository)
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

    [Authorize(Roles = Roles.COMPANYMEMBERS)]
    [HttpGet("my-requests")]
    public async Task<IActionResult> GetMyRequestedTickets()
    {
        var result = await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(CurrentUserID));
        return Ok(result);
    }

    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTicketsByUser(int userId)
    {
        var result = await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId));
        if (result.Count == 0)
        {
            throw new NotFoundException("No tickets was found for this user");
        }
        return Ok(result);
    }

    [Authorize(Roles = Roles.TICKETPARTICIPANTS)]
    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetTicketById(int ticketId)
    {
        var result = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        return Ok(result);
    }

    [Authorize(Roles = $"{Roles.CUSTOMERADMIN},{Roles.CUSTOMER}")]
    [HttpPost("new-ticket")]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest model)
    {
        Ticket entity = Mapper.Map(model, new Ticket());
        //Logic
        entity.RequesterId = CurrentUserID;
        entity.TicketStatus = TicketStatus.Open;
        //Create
        await _ticketRepository.CreateAsync(entity);
        return Ok("Create Successfully");
    }

    [Authorize(Roles = $"{Roles.CUSTOMERADMIN},{Roles.CUSTOMER}")]
    [HttpPut("{ticketId}")]
    public async Task<IActionResult> UpdateTicketInformation(int ticketId, [FromBody]UpdateTicketRequest model)
    {
        var target = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        if (target.TicketStatus == TicketStatus.Open)
        {
            throw new BadRequestException("Ticket can not be updated when it is being executed");
        }
        var entity = Mapper.Map(model, new Ticket());
        await _ticketRepository.UpdateAsync(entity);
        return Accepted(entity);
    }

    //Chưa làm Author
    [Authorize]
    [HttpDelete("{ticketId}")]
    public async Task<IActionResult> DeleteTicket(int ticketId, [FromBody] UpdateTicketRequest model)
    {
        var target = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        await _ticketRepository.DeleteAsync(target);
        return Accepted(target);
    }
}


