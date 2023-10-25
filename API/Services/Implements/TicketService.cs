using API.DTOs.Requests.Tickets;
using API.DTOs.Responses.Tickets;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Hangfire;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class TicketService : ITicketService
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IRepositoryBase<TicketTask> _taskRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IMapper _mapper;

    public TicketService(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IRepositoryBase<TicketTask> taskRepository, IRepositoryBase<User> userRepository, IAuditLogService auditLogService, IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _taskRepository = taskRepository;
        _userRepository = userRepository;
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

        var filteredResult = result.Where(x => !IsTicketDone(x));

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

        var filteredResult = result.Where(x => IsTicketDone(x));

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
        await _ticketRepository.SoftDeleteAsync(target);
    }

    public async Task<Ticket> UpdateByManager(int id, UpdateTicketManagerRequest model)
    {
        var target =
            await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException();
        if (target.TicketStatus != TicketStatus.Open || target.TicketStatus != TicketStatus.Assigned)
        {
            throw new BadRequestException("Ticket can not be updated when it is being executed");
        }
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

    public bool IsTicketDone(Ticket ticket)
    {
        return ticket.TicketStatus is TicketStatus.Closed or TicketStatus.Cancelled;
    }

    public async Task UpdateTicketStatus(int ticketId, TicketStatus newStatus)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(c => c.Id.Equals(ticketId)) ?? throw new KeyNotFoundException();
        ticket.TicketStatus = newStatus;
        await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task UpdateTicketStatusForTechnician(int ticketId, TicketStatus newStatus)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId) ?? throw new KeyNotFoundException();
        var tasks = await _taskRepository.WhereAsync(x => x.TicketId.Equals(ticket.Id));
        var taskIncompletedCount = 0;
        if (tasks.Count > 0)
        {
            taskIncompletedCount = tasks.Count(x =>
            x.TaskStatus != TicketTaskStatus.Completed ||
            x.TaskStatus != TicketTaskStatus.Cancelled);
        }
        if (ticket.TicketStatus != TicketStatus.Open)
        {
            switch (ticket.TicketStatus)
            {
                case TicketStatus.Assigned:
                    if (newStatus == TicketStatus.InProgress)
                    {
                        ticket.TicketStatus = newStatus;
                        await _ticketRepository.UpdateAsync(ticket);
                    }
                    if (newStatus != TicketStatus.Resolved && taskIncompletedCount == 0)
                    {
                        ticket.TicketStatus = newStatus;
                        await _ticketRepository.UpdateAsync(ticket);
                    }
                    else
                    {
                        throw new BadRequestException("Cannot resovle ticket if all the tasks are not completed");
                    }
                    break;
                case TicketStatus.InProgress:
                    if (newStatus == TicketStatus.Resolved)
                    {
                        ticket.TicketStatus = newStatus;
                        await _ticketRepository.UpdateAsync(ticket);
                    }
                    break;
                case TicketStatus.Resolved:
                    if (newStatus == TicketStatus.Closed)
                    {
                        string jobId = BackgroundJob.Schedule(
                            () => CloseTicketJob(ticket.Id),
                            TimeSpan.FromDays(2));
                        RecurringJob.AddOrUpdate(
                            jobId + "_Cancellation",
                            () => CancelCloseTicketJob(jobId, ticket.Id),
                            "*/5 * * * * *"); //Every 5
                    }
                    if (newStatus == TicketStatus.InProgress)
                    {
                        ticket.TicketStatus = newStatus;
                        await _ticketRepository.UpdateAsync(ticket);
                    }
                    break;
                case TicketStatus.Closed:
                    // Handle re-open logic if needed
                    // You can add re-open logic here if you want to allow re-opening of closed tickets.
                    break;
                case TicketStatus.Cancelled:
                    break;
                default:
                    throw new BadRequestException();
            }
        }
    }

    public async Task AssignSupportJob(int ticketId)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
        {
            // Handle the case where the ticket does not exist
            return;
        }

        var availableTechnicians = await _userRepository.WhereAsync(x => x.Role == Role.Technician && x.IsActive == true);

        if (!availableTechnicians.Any())
        {
            // Handle the case where no technicians are available
            return;
        }

        int selectedTechnician = -1;
        int minValue = int.MaxValue;

        foreach (var technician in availableTechnicians)
        {
            int count = await GetNumberOfAssignmentsForTechnician(technician.Id);
            if (count < minValue)
            {
                minValue = count;
                selectedTechnician = technician.Id;
            }
        }

        if (selectedTechnician != -1)
        {
            var assignment = new Assignment()
            {
                TicketId = ticketId,
                TechnicianId = selectedTechnician
            };

            await _assignmentRepository.CreateAsync(assignment);
            await UpdateTicketStatus(ticketId, TicketStatus.Assigned);
        }
        else
        {
            // Handle the case where no technician is available
            // You might want to log this or take other actions
        }
    }

    public async Task CancelAssignSupportJob(string jobId, int ticketId)
    {
        if (await IsTicketAssigned(ticketId) == true)
        {
            BackgroundJob.Delete(jobId);
        }
    }

    public async Task CloseTicketJob(int ticketId)
    {
        await UpdateTicketStatus(ticketId, TicketStatus.Closed);
    }

    public async Task CancelCloseTicketJob(string jobId, int ticketId)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(ticketId));
        if (ticket.TicketStatus == TicketStatus.Closed)
        {
            BackgroundJob.Delete(jobId);
        }

    }

    private async Task<int> GetNumberOfAssignmentsForTechnician(int technicianId)
    {
        var result = await _assignmentRepository.WhereAsync(x => x.TechnicianId == technicianId);
        return result.Count;
    }

}
