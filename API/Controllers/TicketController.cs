using API.DTOs.Requests.Tickets;
using API.DTOs.Responses.Tickets;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models.Tickets;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Persistence.Context;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using X.PagedList;

namespace API.Controllers;

[Route("/v1/itsds/ticket")]
public class TicketController : BaseController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IStatusTrackingService _statusTrackingService;
    private readonly IAuditLogService _auditLogService;

    public TicketController(ApplicationDbContext dbContext, IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IStatusTrackingService statusTrackingService, IAuditLogService auditLogService)
    {
        _dbContext = dbContext;
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _statusTrackingService = statusTrackingService;
        _auditLogService = auditLogService;
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

    [Authorize]
    [HttpGet("log")]
    public async Task<IActionResult> GetTicketHistories(int ticketId)
    {
        try
        {
            var histories = await _dbContext.AuditLogs
                .Where(history => history.EntityName.Equals(Tables.TICKET) && history.EntityRowId == ticketId)
                .Include(history => history.User)
                .ToListAsync();

            var groupedHistories = histories
                .GroupBy(history => new { history.Timestamp, history.User.Username, history.Action })
                .OrderByDescending(group => group.Key.Timestamp)
                .Select(group => new
                {
                    group.Key.Timestamp,
                    group.Key.Username,
                    group.Key.Action,
                    Entries = group.Select(entry => new
                    {
                        entry.Id,
                        entry.Message
                    }).ToList()
                }).ToList();

            return Ok(groupedHistories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while processing your request.", Details = ex.Message });
        }
    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpPost("customer/new")]
    public async Task<IActionResult> CreateTicketByCustomer([FromBody] CreateTicketCustomerRequest model)
    {
        Ticket entity = Mapper.Map(model, new Ticket());
        //Logic
        entity.TicketStatus = TicketStatus.Open;
        entity.RequesterId = CurrentUserID;
        //Create
        await _ticketRepository.CreateAsync(entity);
        await _auditLogService.TrackCreated(entity.Id, Tables.TICKET, CurrentUserID);
        return Ok("Create Successfully");
    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpPut("customer/{ticketId}")]
    public async Task<IActionResult> UpdateTicketByCustomer(int ticketId, [FromBody] UpdateTicketCustomerRequest model)
    {
        var target = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(ticketId)) ?? throw new BadRequestException("Not Found");
        if (target.TicketStatus != TicketStatus.Open)
        {
            throw new BadRequestException("Ticket can not be updated when it is being executed");
        }

        // Create a deep copy of the target entity
        var original = JsonConvert.DeserializeObject<Ticket>(JsonConvert.SerializeObject(target));

        // Map your model to the target entity
        var entity = Mapper.Map(model, target);

        await _ticketRepository.UpdateAsync(entity);
        await _auditLogService.TrackUpdated(original, entity, CurrentUserID, ticketId, Tables.TICKET);
        return Ok("Update Successfully");
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
        await _auditLogService.TrackCreated(entity.Id, Tables.TICKET, CurrentUserID);
        return Ok("Create Successfully");
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPut("manager/{ticketId}")]
    public async Task<IActionResult> UpdateTicketByManager(int ticketId, [FromBody] UpdateTicketManagerRequest model)
    {
        var target =
            await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));

        // Create a deep copy of the target entity
        var original = JsonConvert.DeserializeObject<Ticket>(JsonConvert.SerializeObject(target));

        var entity = Mapper.Map(model, target);
        await _ticketRepository.UpdateAsync(entity);
        await _auditLogService.TrackUpdated(original, entity, CurrentUserID, ticketId, Tables.TICKET);
        return Ok(entity);
    }

    //Chưa làm Author
    [Authorize(Roles = Roles.MANAGER)]
    [HttpDelete("manager/{ticketId}")]
    public async Task<IActionResult> DeleteTicket(int ticketId)
    {
        var target = await _ticketRepository.FoundOrThrow(x => x.Id.Equals(ticketId), new NotFoundException("Ticket not found"));
        await _ticketRepository.DeleteAsync(target);
        return Ok(target);
    }
}