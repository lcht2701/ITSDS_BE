using API.DTOs.Requests.Tickets;
using API.DTOs.Responses.Tickets;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models.Tickets;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class TicketService : ITicketService
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IStatusTrackingService _statusTrackingService;
    private readonly IAuditLogService _auditLogService;
    private readonly IMapper _mapper;

    public TicketService(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IStatusTrackingService statusTrackingService, IAuditLogService auditLogService, IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _statusTrackingService = statusTrackingService;
        _auditLogService = auditLogService;
        _mapper = mapper;
    }

    public async Task<Ticket> CreateByCustomer(int userId, CreateTicketCustomerRequest model)
    {
        Ticket entity = _mapper.Map(model, new Ticket());
        entity.RequesterId = userId;
        entity.TicketStatus = TicketStatus.Open;
        await _ticketRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<Ticket> CreateByManager(CreateTicketManagerRequest model)
    {
        Ticket entity = _mapper.Map(model, new Ticket());
        entity.TicketStatus = TicketStatus.Open;
        await _ticketRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<List<GetTicketResponse>> Get()
    {
        var result = await _ticketRepository.GetAsync(navigationProperties: new string[]
        { "Requester", "Service", "Category", "Mode" });

        var response = result.Select(ticket =>
        {
            var entity = _mapper.Map(ticket, new GetTicketResponse());
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        }).ToList();
        return response;
    }

    public async Task<List<GetTicketResponse>> GetAssignedTickets(int userId)
    {
        var assignments = await _assignmentRepository.WhereAsync(x => x.TechnicianId == userId);

        var ticketIds = assignments.Select(assignment => assignment.TicketId).ToList();
        var result = await _ticketRepository.WhereAsync(ticket => ticketIds.Contains(ticket.Id), new string[] { "Requester", "Service", "Category", "Mode" });

        // Map the tickets to response entities
        var response = result.Select(ticket =>
        {
            var entity = _mapper.Map<GetTicketResponse>(ticket);
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        })
        .OrderByDescending(x => x.CreatedAt)
        .ToList();
        return response;
    }

    public async Task<object> GetById(int id)
    {
        var result =
             await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id),
                 new string[] { "Requester", "Service", "Category", "Mode" }) ?? throw new KeyNotFoundException(); ;
        _mapper.Map<GetTicketResponse>(result);
        DataResponse.CleanNullableDateTime(result);
        return result;
    }

    public async Task<List<GetTicketResponse>> GetByUser(int userId)
    {
        var result = await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId),
            new string[] { "Requester", "Service", "Category", "Mode" });
        var response = result.Select(ticket =>
        {
            var entity = _mapper.Map(ticket, new GetTicketResponse());
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        }).ToList();
        return response;
    }

    public async Task<List<GetTicketResponse>> GetTicketAvailable(int userId)
    {
        var result = await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId),
        new string[] { "Requester", "Service", "Category", "Mode" });

        var filteredResult = result.Where(x => !_statusTrackingService.isTicketDone(x));

        var response = filteredResult.Select(ticket =>
        {
            var entity = _mapper.Map<GetTicketResponse>(ticket);
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        })
        .OrderByDescending(x => x.CreatedAt)
        .ToList();
        return response;
    }

    public async Task<List<GetTicketResponse>> GetTicketHistory(int userId)
    {
        var result = await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId),
        new string[] { "Requester", "Service", "Category", "Mode" });

        var filteredResult = result.Where(x => _statusTrackingService.isTicketDone(x));

        var response = filteredResult.Select(ticket =>
        {
            var entity = _mapper.Map<GetTicketResponse>(ticket);
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        })
        .OrderByDescending(x => x.CompletedTime)
        .ToList();
        return response;
    }

    public async Task<object> GetTicketLog(int id)
    {
        var result = await _auditLogService.GetLog(id, Tables.TICKET);
        return result;
    }

    public async Task<bool> IsTicketAssigned(int ticketId)
    {
        return await _assignmentRepository.FirstOrDefaultAsync(o => o.TicketId == ticketId) != null;
    }

    public async Task Remove(int id)
    {
        var target = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id));
        await _ticketRepository.DeleteAsync(target);
    }

    public async Task<Ticket> UpdateByManager(int id, UpdateTicketManagerRequest model)
    {
        var target =
            await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException();
        //if (target.TicketStatus != TicketStatus.Open || target.TicketStatus != TicketStatus.Assigned)
        //{
        //    throw new BadRequestException("Ticket can not be updated when it is being executed");
        //}
        var result = _mapper.Map(model, target);
        await _ticketRepository.UpdateAsync(result);
        return result;
    }

    public async Task<Ticket> UpdateByCustomer(int id, UpdateTicketCustomerRequest model)
    {
        var target =
            await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException();
        var result = _mapper.Map(model, target);
        await _ticketRepository.UpdateAsync(result);
        return result;
    }
}
