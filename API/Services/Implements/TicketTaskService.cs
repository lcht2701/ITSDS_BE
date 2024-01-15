using API.DTOs.Requests.TicketTasks;
using API.DTOs.Responses.TicketTasks;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants;
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
    private readonly IAttachmentService _attachmentService;
    private readonly IMapper _mapper;

    public TicketTaskService(IRepositoryBase<TicketTask> taskRepository, IRepositoryBase<Ticket> ticketRepository,
        IRepositoryBase<Assignment> assignmentRepository, ITicketService ticketService, IAttachmentService attachmentService, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _ticketService = ticketService;
        _attachmentService = attachmentService;
        _mapper = mapper;
    }

    public async Task<List<GetTicketTaskResponse>> Get(int ticketId)
    {
        var result = await _taskRepository.WhereAsync(x => x.TicketId.Equals(ticketId),
            new string[] { "CreateBy", "Ticket" });
        var response = _mapper.Map(result, new List<GetTicketTaskResponse>());
        foreach (var entity in response)
        {
            DataResponse.CleanNullableDateTime(entity);
        }
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
                    new string[] { "CreateBy", "Ticket" });
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
            var ticketIds = (await _assignmentRepository
                .WhereAsync(x => x.TechnicianId == userId)).Select(assignment => assignment.TicketId);

            var ticketList = await _ticketRepository
                .WhereAsync(ticket => ticketIds.Contains(ticket.Id) &&
                                      (ticket.TicketStatus == TicketStatus.Closed ||
                                       ticket.TicketStatus == TicketStatus.Cancelled));

            foreach (var ticket in ticketList)
            {
                var tasks = await _taskRepository
                    .WhereAsync(x => x.TicketId == ticket.Id,
                new string[] { "CreateBy", "Ticket" });
                taskList.AddRange(tasks);
            }
            response = _mapper.Map<List<GetTicketTaskResponse>>(taskList);
        }
        return response;
    }

    public async Task<GetTicketTaskResponse> GetById(int id)
    {
        var result = await _taskRepository.FirstOrDefaultAsync(x => x.Id.Equals(id),
            new string[] { "CreateBy", "Ticket" }) ?? throw new KeyNotFoundException("Task is not exist");
        var response = _mapper.Map(result, new GetTicketTaskResponse());
        DataResponse.CleanNullableDateTime(response);
        response.AttachmentUrls = (await _attachmentService.Get(Tables.TICKETTASK, (int)response.Id!)).Select(x => x.Url).ToList();
        return response;
    }

    public async Task Create(CreateTicketTaskRequest model, int createdById)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(model.TicketId)) ??
                     throw new KeyNotFoundException();
        var entity = _mapper.Map(model, new TicketTask());
        entity.CreateById = createdById;
        entity.TaskStatus = TicketTaskStatus.New;
        if (model.ScheduledStartTime == null) entity.ScheduledStartTime = DateTime.Now;
        var tasksCount = await Get((int)model.TicketId!);
        if (ticket.TicketStatus == TicketStatus.Assigned)
        {
            await _ticketService.UpdateTicketStatus((int)model.TicketId, TicketStatus.InProgress);
        }
        var result = await _taskRepository.CreateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Add(Tables.TICKETTASK, result.Id, model.AttachmentUrls);
        }

    }

    public async Task Update(int taskId, UpdateTicketTaskRequest model)
    {
        var target = await _taskRepository.FirstOrDefaultAsync(c => c.Id.Equals(taskId)) ??
                     throw new KeyNotFoundException();
        TicketTask entity = _mapper.Map(model, target);
        var result = await _taskRepository.UpdateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Add(Tables.TICKETTASK, result.Id, model.AttachmentUrls);
        }
    }

    public async Task Remove(int taskId)
    {
        var target = await _taskRepository.FirstOrDefaultAsync(c => c.Id.Equals(taskId)) ??
                     throw new KeyNotFoundException();
        await _attachmentService.Delete(Tables.TICKET, target.Id);
        await _taskRepository.SoftDeleteAsync(target);
    }

    public async Task UpdateTaskStatus(int taskId, TicketTaskStatus newStatus)
    {
        var target = await _taskRepository.FirstOrDefaultAsync(c => c.Id.Equals(taskId)) ??
                     throw new KeyNotFoundException();
        target.TaskStatus = newStatus;
        switch (newStatus)
        {
            case TicketTaskStatus.InProgress:
                {
                    if (target.ActualStartTime == null)
                    {
                        target.ActualStartTime = DateTime.Now;
                    }
                    target.ActualEndTime = null;
                    break;
                }
            case TicketTaskStatus.Completed:
            case TicketTaskStatus.Cancelled:
                {
                    if (target.ActualStartTime == null)
                    {
                        target.ActualStartTime = DateTime.Now;
                    }
                    target.ActualEndTime = DateTime.Now;
                    break;
                }
        }
        await _taskRepository.UpdateAsync(target);
    }
}