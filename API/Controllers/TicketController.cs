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

    [Authorize]
    [HttpGet("team/{teamId}")]
    public async Task<IActionResult> GetTicketsByTeam(int teamId)
    {
        var result = await _ticketRepository.WhereAsync(x => x.TeamId.Equals(teamId));
        if (result.Count == 0)
        {
            throw new NotFoundException("No tickets was found for this team");
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

    [Authorize(Roles = Roles.COMPANYMEMBERS)]
    [HttpPost("customer/new")]
    public async Task<IActionResult> CreateTicketByCustomer([FromBody] CreateTicketCustomerRequest model)
    {
        Ticket entity = Mapper.Map(model, new Ticket());
        //Logic
        entity.TicketStatus = TicketStatus.Open;
        entity.RequesterId = CurrentUserID;
        //Create
        await _ticketRepository.CreateAsync(entity);
        return Ok("Create Successfully");
    }

    [Authorize(Roles = Roles.COMPANYMEMBERS)]
    [HttpPut("customer/{ticketId}")]
    public async Task<IActionResult> UpdateTicketByCustomer(int ticketId, [FromBody] UpdateTicketCustomerRequest model)
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


    [Authorize(Roles = Roles.MANAGER)]
    [HttpPost("manager/new")]
    public async Task<IActionResult> CreateTicketByManager([FromBody] CreateTicketManagerRequest model)
    {
        Ticket entity = Mapper.Map(model, new Ticket());
        //Logic
        entity.TicketStatus = TicketStatus.Open;
        //Create
        await _ticketRepository.CreateAsync(entity);
        return Ok("Create Successfully");
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPut("manager/{ticketId}")]
    public async Task<IActionResult> UpdateTicketByManager(int ticketId, [FromBody] UpdateTicketManagerRequest model)
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
    [Authorize(Roles = Roles.MANAGER)]
    [HttpDelete("manager/{ticketId}")]
    public async Task<IActionResult> DeleteTicket(int ticketId)
    {
        var target = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        await _ticketRepository.DeleteAsync(target);
        return Accepted(target);
    }
}


