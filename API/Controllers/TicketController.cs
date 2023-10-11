using API.DTOs.Requests.Tickets;
using API.DTOs.Responses.Tickets;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/ticket")]
public class TicketController : BaseController
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Team> _teamRepository;

    public TicketController(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<User> userRepository,
        IRepositoryBase<Team> teamRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var result = await _ticketRepository.GetAsync(navigationProperties: new string[] { "Category", "Mode" });
        var response = new List<GetTicketResponse>();
        foreach (var ticket in result)
        {
            var entity = Mapper.Map(ticket, new GetTicketResponse());
            entity.TicketStatus = EnumExtensions.GetEnumDescription(ticket.TicketStatus!);
            entity.Impact = EnumExtensions.GetEnumDescription(ticket.Impact!);
            entity.Priority = EnumExtensions.GetEnumDescription(ticket.Priority!);
            entity.Urgency = EnumExtensions.GetEnumDescription(ticket.Urgency!);

            entity.ScheduledStartTime = (ticket.ScheduledStartTime == DateTime.MinValue) ? null : ticket.ScheduledStartTime;
            entity.ScheduledEndTime = (ticket.ScheduledEndTime == DateTime.MinValue) ? null : ticket.ScheduledEndTime;
            entity.DueTime = (ticket.DueTime == DateTime.MinValue) ? null : ticket.DueTime;
            entity.CompletedTime = (ticket.CompletedTime == DateTime.MinValue) ? null : ticket.CompletedTime;
            entity.CreatedAt = (ticket.CreatedAt == DateTime.MinValue) ? null : ticket.CreatedAt;
            entity.ModifiedAt = (ticket.ModifiedAt == DateTime.MinValue) ? null : ticket.ModifiedAt;
            entity.DeletedAt = (ticket.DeletedAt == DateTime.MinValue) ? null : ticket.DeletedAt;
            response.Add(entity);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTicketsByUser(int userId)
    {
        var result = await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId));
        var response = new List<GetTicketResponse>();
        foreach (var ticket in result)
        {
            var entity = Mapper.Map(ticket, new GetTicketResponse());
            entity.ScheduledStartTime = (ticket.ScheduledStartTime == DateTime.MinValue) ? null : ticket.ScheduledStartTime;
            entity.ScheduledEndTime = (ticket.ScheduledEndTime == DateTime.MinValue) ? null : ticket.ScheduledEndTime;
            entity.DueTime = (ticket.DueTime == DateTime.MinValue) ? null : ticket.DueTime;
            entity.CompletedTime = (ticket.CompletedTime == DateTime.MinValue) ? null : ticket.CompletedTime;
            entity.CreatedAt = (ticket.CreatedAt == DateTime.MinValue) ? null : ticket.CreatedAt;
            entity.ModifiedAt = (ticket.ModifiedAt == DateTime.MinValue) ? null : ticket.ModifiedAt;
            entity.DeletedAt = (ticket.DeletedAt == DateTime.MinValue) ? null : ticket.DeletedAt;
            response.Add(entity);
        }

        return Ok(response);
    }

    // [Authorize(Roles = Roles.TICKETPARTICIPANTS)]
    [Authorize]
    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetTicketById(int ticketId)
    {
        var result =
            await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));

        var entity = Mapper.Map(result, new GetTicketResponse());
        entity.ScheduledStartTime = (result.ScheduledStartTime == DateTime.MinValue) ? null : result.ScheduledStartTime;
        entity.ScheduledEndTime = (result.ScheduledEndTime == DateTime.MinValue) ? null : result.ScheduledEndTime;
        entity.DueTime = (result.DueTime == DateTime.MinValue) ? null : result.DueTime;
        entity.CompletedTime = (result.CompletedTime == DateTime.MinValue) ? null : result.CompletedTime;
        entity.CreatedAt = (result.CreatedAt == DateTime.MinValue) ? null : result.CreatedAt;
        entity.ModifiedAt = (result.ModifiedAt == DateTime.MinValue) ? null : result.ModifiedAt;
        entity.DeletedAt = (result.DeletedAt == DateTime.MinValue) ? null : result.DeletedAt;

        return Ok(entity);
    }

    [Authorize(Roles = Roles.CUSTOMER + "," + Roles.ADMIN)]
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

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpPut("customer/{ticketId}")]
    public async Task<IActionResult> UpdateTicketByCustomer(int ticketId, [FromBody] UpdateTicketCustomerRequest model)
    {
        var target =
            await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
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
        var target =
            await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
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
        var target =
            await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        await _ticketRepository.DeleteAsync(target);
        return Accepted(target);
    }
}