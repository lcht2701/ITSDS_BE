using API.DTOs.Requests.Tickets;
using API.DTOs.Responses.Tickets;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using Persistence.Services.Interfaces;
using X.PagedList;

namespace API.Controllers;

[Route("/v1/itsds/ticket")]
public class TicketController : BaseController
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IStatusTrackingService _statusTrackingService;

    public TicketController(IRepositoryBase<Ticket> ticketRepository, IStatusTrackingService statusTrackingService)
    {
        _ticketRepository = ticketRepository;
        _statusTrackingService = statusTrackingService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTickets(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var result = await _ticketRepository.GetAsync(navigationProperties: new string[]
        { "Requester", "Assignment", "Service", "Category", "Mode" });

        var response = result.Select(ticket =>
        {
            var entity = Mapper.Map(ticket, new GetTicketResponse());
            entity.TicketStatus = EnumExtensions.GetEnumDescription(ticket.TicketStatus!);
            entity.Impact = EnumExtensions.GetEnumDescription(ticket.Impact!);
            entity.Priority = EnumExtensions.GetEnumDescription(ticket.Priority!);
            entity.Urgency = EnumExtensions.GetEnumDescription(ticket.Urgency!);

            entity.ScheduledStartTime = ticket.ScheduledStartTime == DateTime.MinValue ? null : ticket.ScheduledStartTime;
            entity.ScheduledEndTime = ticket.ScheduledEndTime == DateTime.MinValue ? null : ticket.ScheduledEndTime;
            entity.DueTime = ticket.DueTime == DateTime.MinValue ? null : ticket.DueTime;
            entity.CompletedTime = ticket.CompletedTime == DateTime.MinValue ? null : ticket.CompletedTime;
            entity.CreatedAt = ticket.CreatedAt == DateTime.MinValue ? null : ticket.CreatedAt;
            entity.ModifiedAt = ticket.ModifiedAt == DateTime.MinValue ? null : ticket.ModifiedAt;
            entity.DeletedAt = ticket.DeletedAt == DateTime.MinValue ? null : ticket.DeletedAt;

            return entity;
        }).ToList();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            response = response.AsQueryable().Filter(filter).ToList();
        }

        var pagedResponse = response.AsQueryable().GetPagedData(page, pageSize, sort);

        return Ok(pagedResponse);
    }


    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTicketsOfUser(int userId,
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var result = await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId),
            new string[] { "Requester", "Assignment", "Service", "Category", "Mode" });
        var response = result.Select(ticket =>
        {
            var entity = Mapper.Map(ticket, new GetTicketResponse());
            entity.TicketStatus = EnumExtensions.GetEnumDescription(ticket.TicketStatus!);
            entity.Impact = EnumExtensions.GetEnumDescription(ticket.Impact!);
            entity.Priority = EnumExtensions.GetEnumDescription(ticket.Priority!);
            entity.Urgency = EnumExtensions.GetEnumDescription(ticket.Urgency!);

            entity.ScheduledStartTime = CleanNullableDateTime(entity.ScheduledStartTime);
            entity.ScheduledEndTime = CleanNullableDateTime(entity.ScheduledEndTime);
            entity.DueTime = CleanNullableDateTime(entity.DueTime);
            entity.CompletedTime = CleanNullableDateTime(entity.CompletedTime);
            entity.CreatedAt = CleanNullableDateTime(entity.CreatedAt);
            entity.ModifiedAt = CleanNullableDateTime(entity.ModifiedAt);
            entity.DeletedAt = CleanNullableDateTime(entity.DeletedAt);

            return entity;
        }).ToList();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            response = response.AsQueryable().Filter(filter).ToList();
        }

        var pagedResponse = response.AsQueryable().GetPagedData(page, pageSize, sort);

        return Ok(pagedResponse);
    }

    [Authorize]
    [HttpGet("user/{userId}/history")]
    public async Task<IActionResult> GetTicketHistoryOfUser(int userId)
    {
        var result = await _ticketRepository.WhereAsync(x =>
            x.RequesterId == userId && _statusTrackingService.isTicketDone(x),
            new string[] { "Requester", "Assignment", "Service", "Category", "Mode" });

        var response = result.Select(ticket =>
        {
            var entity = Mapper.Map<GetTicketResponse>(ticket);
            entity.TicketStatus = EnumExtensions.GetEnumDescription(ticket.TicketStatus!);
            entity.Impact = EnumExtensions.GetEnumDescription(ticket.Impact!);
            entity.Priority = EnumExtensions.GetEnumDescription(ticket.Priority!);
            entity.Urgency = EnumExtensions.GetEnumDescription(ticket.Urgency!);

            entity.ScheduledStartTime = CleanNullableDateTime(entity.ScheduledStartTime);
            entity.ScheduledEndTime = CleanNullableDateTime(entity.ScheduledEndTime);
            entity.DueTime = CleanNullableDateTime(entity.DueTime);
            entity.CompletedTime = CleanNullableDateTime(entity.CompletedTime);
            entity.CreatedAt = CleanNullableDateTime(entity.CreatedAt);
            entity.ModifiedAt = CleanNullableDateTime(entity.ModifiedAt);
            entity.DeletedAt = CleanNullableDateTime(entity.DeletedAt);

            return entity;
        })
        .OrderByDescending(x => x.CompletedTime)
        .ToList();

        return Ok(response);
    }

    [Authorize]
    [HttpGet("user/{userId}/available")]
    public async Task<IActionResult> GetAvailableTicketsOfUser(int userId)
    {
        var result = await _ticketRepository.WhereAsync(x =>
            x.RequesterId == userId && !_statusTrackingService.isTicketDone(x),
            new string[] { "Requester", "Assignment", "Service", "Category", "Mode" });

        var response = result.Select(ticket =>
        {
            var entity = Mapper.Map<GetTicketResponse>(ticket);
            entity.TicketStatus = EnumExtensions.GetEnumDescription(ticket.TicketStatus!);
            entity.Impact = EnumExtensions.GetEnumDescription(ticket.Impact!);
            entity.Priority = EnumExtensions.GetEnumDescription(ticket.Priority!);
            entity.Urgency = EnumExtensions.GetEnumDescription(ticket.Urgency!);

            entity.ScheduledStartTime = CleanNullableDateTime(entity.ScheduledStartTime);
            entity.ScheduledEndTime = CleanNullableDateTime(entity.ScheduledEndTime);
            entity.DueTime = CleanNullableDateTime(entity.DueTime);
            entity.CompletedTime = CleanNullableDateTime(entity.CompletedTime);
            entity.CreatedAt = CleanNullableDateTime(entity.CreatedAt);
            entity.ModifiedAt = CleanNullableDateTime(entity.ModifiedAt);
            entity.DeletedAt = CleanNullableDateTime(entity.DeletedAt);

            return entity;
        })
        .OrderByDescending(x => x.CreatedAt)
        .ToList();

        return Ok(response);
    }

    [Authorize]
    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetTicketById(int ticketId)
    {
        var result =
            await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(ticketId),
                new string[] { "Requester", "Assignment", "Service", "Category", "Mode" });

        var entity = Mapper.Map(result, new GetTicketResponse());
        entity.ScheduledStartTime = CleanNullableDateTime(entity.ScheduledStartTime);
        entity.ScheduledEndTime = CleanNullableDateTime(entity.ScheduledEndTime);
        entity.DueTime = CleanNullableDateTime(entity.DueTime);
        entity.CompletedTime = CleanNullableDateTime(entity.CompletedTime);
        entity.CreatedAt = CleanNullableDateTime(entity.CreatedAt);
        entity.ModifiedAt = CleanNullableDateTime(entity.ModifiedAt);
        entity.DeletedAt = CleanNullableDateTime(entity.DeletedAt);

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

    private DateTime? CleanNullableDateTime(DateTime? dateTime)
    {
        return dateTime == DateTime.MinValue ? null : dateTime;
    }

}