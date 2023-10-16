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
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IStatusTrackingService _statusTrackingService;

    public TicketController(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IStatusTrackingService statusTrackingService)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
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
        { "Requester", "Service", "Category", "Mode" });

        var response = result.Select(ticket =>
        {
            var entity = Mapper.Map(ticket, new GetTicketResponse());
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        }).ToList();

        var pagedResponse = response.AsQueryable().GetPagedData(page, pageSize, filter, sort);

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
            new string[] { "Requester", "Service", "Category", "Mode" });
        var response = result.Select(ticket =>
        {
            var entity = Mapper.Map(ticket, new GetTicketResponse());
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        }).ToList();

        var pagedResponse = response.AsQueryable().GetPagedData(page, pageSize, filter, sort);

        return Ok(pagedResponse);
    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpGet("user/history")]
    public async Task<IActionResult> GetTicketHistoryOfCurrentUser()
    {
        var result = await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(CurrentUserID),
        new string[] { "Requester", "Service", "Category", "Mode" });

        var filteredResult = result.Where(x => _statusTrackingService.isTicketDone(x));

        var response = filteredResult.Select(ticket =>
        {
            var entity = Mapper.Map<GetTicketResponse>(ticket);
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        })
        .OrderByDescending(x => x.CompletedTime)
        .ToList();

        return Ok(response);

    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpGet("user/available")]
    public async Task<IActionResult> GetAvailableTicketsOfCurrentUser()
    {
        var result = await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(CurrentUserID),
        new string[] { "Requester", "Service", "Category", "Mode" });

        var filteredResult = result.Where(x => !_statusTrackingService.isTicketDone(x));

        var response = filteredResult.Select(ticket =>
        {
            var entity = Mapper.Map<GetTicketResponse>(ticket);
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        })
        .OrderByDescending(x => x.CreatedAt)
        .ToList();

        return Ok(response);
    }

    [Authorize(Roles = Roles.TECHNICIAN)]
    [HttpGet("assign")]
    public async Task<IActionResult> GetAssignedTickets()
    {
        var assignments = await _assignmentRepository.WhereAsync(x => x.TechnicianId == CurrentUserID);

        if (!assignments.Any())
        {
            return Ok("You have not been assigned");
        }

        var ticketIds = assignments.Select(assignment => assignment.TicketId).ToList();
        var result = await _ticketRepository.WhereAsync(ticket => ticketIds.Contains(ticket.Id), new string[] { "Requester", "Service", "Category", "Mode" });

        // Map the tickets to response entities
        var response = result.Select(ticket =>
        {
            var entity = Mapper.Map<GetTicketResponse>(ticket);
            DataResponse.CleanNullableDateTime(entity);
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
        var ticket =
            await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(ticketId),
                new string[] { "Requester", "Service", "Category", "Mode" });

        var entity = Mapper.Map<GetTicketResponse>(ticket);
        DataResponse.CleanNullableDateTime(entity);
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