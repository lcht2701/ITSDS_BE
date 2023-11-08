using API.DTOs.Requests.TicketTasks;
using API.DTOs.Responses.TicketTasks;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class TicketTaskService : ITicketTaskService
{
    private readonly IRepositoryBase<TicketTask> _taskRepository;
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public TicketTaskService(IRepositoryBase<TicketTask> taskRepository, IRepositoryBase<Ticket> ticketRepository,
        IRepositoryBase<Assignment> assignmentRepository, ITicketService ticketService, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<List<GetTicketTaskResponse>> Get(int ticketId)
    {
        var result = await _taskRepository.WhereAsync(x => x.TicketId.Equals(ticketId),
            new string[] { "Technician", "CreateBy", "Team", "Ticket" });
        var response = result.Select(task =>
        {
            var entity = _mapper.Map(task, new GetTicketTaskResponse());
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        }).ToList();
        return response;
    }

    public async Task<List<GetTicketTaskResponse>> GetActiveTasks(int userId, int? ticketId)
    {
        var response = new List<GetTicketTaskResponse>();
        if (ticketId != null)
        {
            response = await Get((int)ticketId);
        }
        else
        {
            var taskList = new List<TicketTask>();
            var assignments = await _assignmentRepository
                .WhereAsync(x => x.TechnicianId == userId);

            var ticketIds = assignments.Select(assignment => assignment.TicketId).ToList();

            var ticketList = await _ticketRepository
                .WhereAsync(ticket => ticketIds.Contains(ticket.Id) &&
                                      (ticket.TicketStatus != TicketStatus.Closed &&
                                       ticket.TicketStatus != TicketStatus.Cancelled));

            foreach (var ticket in ticketList)
            {
                var tasks = await _taskRepository
                    .WhereAsync(x => x.TicketId == ticket.Id,
                    new string[] { "Technician", "CreateBy", "Team", "Ticket" });
                taskList.AddRange(tasks);
            }
            response = _mapper.Map<List<GetTicketTaskResponse>>(taskList);
        }
        return response;
    }

    public async Task<List<GetTicketTaskResponse>> GetInActiveTasks(int userId, int? ticketId)
    {
        var response = new List<GetTicketTaskResponse>();
        if (ticketId != null)
        {
            response = await Get((int)ticketId);
        }
        else
        {
            var taskList = new List<TicketTask>();
            var assignments = (await _assignmentRepository
                .WhereAsync(x => x.TechnicianId == userId));

            var ticketIds = assignments.Select(assignment => assignment.TicketId).ToList();

            var ticketList = await _ticketRepository
                .WhereAsync(ticket => ticketIds.Contains(ticket.Id) &&
                                      (ticket.TicketStatus == TicketStatus.Closed ||
                                       ticket.TicketStatus == TicketStatus.Cancelled));

            foreach (var ticket in ticketList)
            {
                var tasks = await _taskRepository
                    .WhereAsync(x => x.TicketId == ticket.Id,
                new string[] { "Technician", "CreateBy", "Team", "Ticket" });
                taskList.AddRange(tasks);
            }
            response = _mapper.Map<List<GetTicketTaskResponse>>(taskList);
        }
        return response;
    }

    public async Task<GetTicketTaskResponse> GetById(int id)
    {
        var result = await _taskRepository.FirstOrDefaultAsync(x => x.Id.Equals(id),
            new string[] { "Technician", "CreateBy", "Team", "Ticket" }) ?? throw new KeyNotFoundException("Task is not exist");
        var response = _mapper.Map(result, new GetTicketTaskResponse());
        DataResponse.CleanNullableDateTime(response);
        return response;
    }

    public async Task Create(CreateTicketTaskRequest model, int createdBy)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(model.TicketId)) ??
                     throw new KeyNotFoundException();
        var entity = _mapper.Map(model, new TicketTask());
        entity.CreateById = createdBy;
        entity.TaskStatus = TicketTaskStatus.Open;
        if (entity.TechnicianId != null || entity.TeamId != null)
        {
            entity.TaskStatus = TicketTaskStatus.Assigned;
        }

        var tasksCount = await Get((int)model.TicketId!);
        if (tasksCount.Count == 0 && ticket.TicketStatus == TicketStatus.Assigned)
        {
            await _ticketService.UpdateTicketStatus((int)model.TicketId, TicketStatus.InProgress);
        }

        await _taskRepository.CreateAsync(entity);
    }

    public async Task Remove(int taskId)
    {
        var target = await _taskRepository.FirstOrDefaultAsync(c => c.Id.Equals(taskId)) ??
                     throw new KeyNotFoundException();
        await _taskRepository.SoftDeleteAsync(target);
    }

    public async Task Update(int taskId, UpdateTicketTaskRequest model)
    {
        var target = await _taskRepository.FirstOrDefaultAsync(c => c.Id.Equals(taskId)) ??
                     throw new KeyNotFoundException();
        TicketTask entity = _mapper.Map(model, target);
        await _taskRepository.UpdateAsync(entity);
    }

    public async Task UpdateTaskStatus(int taskId, TicketTaskStatus newStatus)
    {
        var target = await _taskRepository.FirstOrDefaultAsync(c => c.Id.Equals(taskId)) ??
                     throw new KeyNotFoundException();
        target.TaskStatus = newStatus;
        if (newStatus == TicketTaskStatus.InProgress && target.ActualStartTime == null)
        {
            target.ActualStartTime = DateTime.Now;
        }
        else if (newStatus == TicketTaskStatus.Completed || newStatus == TicketTaskStatus.Cancelled)
        {
            if (target.ActualStartTime == null)
            {
                target.ActualStartTime = DateTime.Now;
            }
            target.ActualEndTime = DateTime.Now;
        }
        await _taskRepository.UpdateAsync(target);
    }
}