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

    //Chưa biết để role gì
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var result = await _ticketRepository.ToListAsync();
        return Ok(result);
    }

    //Dùng cho customer && CustomerAdmin
    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.CUSTOMERADMIN}")]
    [HttpGet("my-tickets")]
    public async Task<IActionResult> GetTicketOfCurrentUser()
    {
        var result = await _ticketRepository.WhereAsync(x => x.Id.Equals(CurrentUserID));
        if (result.Count == 0)
        {
            throw new NotFoundException("No tickets was found for this user");
        }
        return Ok(result);
    }

    //Chưa sure
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

    //Ngoại trừ Admin + Accountant
    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN},{Roles.CUSTOMER},{Roles.CUSTOMERADMIN}")]
    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetTicketById(int ticketId)
    {
        var result = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        return Ok(result);
    }

    //Dành cho manager và technical để quản lý của team
    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet("team/{teamId}")]
    public async Task<IActionResult> GetTicketsResponsibleByTeam(int teamId)
    {
        var result = await _ticketRepository.WhereAsync(x => x.TeamId.Equals(teamId));
        if (result.Count == 0)
        {
            throw new NotFoundException("No tickets was found for this team");
        }
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest model)
    {
        Ticket entity = Mapper.Map(model, new Ticket());
        //Logic
        entity.TicketStatus = TicketStatus.Open;
        await _ticketRepository.CreateAsync(entity);
        return Ok("Create Successfully");
    }

    //Chưa làm Author
    [Authorize]
    [HttpPut("{ticketId}")]
    public async Task<IActionResult> UpdateTicketInformation(int ticketId, [FromBody]UpdateTicketRequest model)
    {
        var target = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
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


