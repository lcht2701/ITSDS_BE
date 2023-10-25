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
    private readonly IMapper _mapper;

    public TicketTaskService(IRepositoryBase<TicketTask> taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task Create(int ticketId, CreateTicketTaskRequest model, int createdBy)
    {
        var entity = _mapper.Map(model, new TicketTask()) ?? throw new KeyNotFoundException();
        entity.TicketId = ticketId;
        entity.CreateById = createdBy;
        entity.TaskStatus = TicketTaskStatus.Open;
        if (entity.TechnicianId != null || entity.TeamId != null)
        {
            entity.TaskStatus = TicketTaskStatus.Assigned;
        }
        await _taskRepository.CreateAsync(entity);
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

    public async Task Remove(int taskId)
    {
        var target = await _taskRepository.FirstOrDefaultAsync(c => c.Id.Equals(taskId)) ?? throw new KeyNotFoundException();
        await _taskRepository.SoftDeleteAsync(target);
    }

    public async Task Update(int taskId, UpdateTicketTaskRequest model)
    {
        var target = await _taskRepository.FirstOrDefaultAsync(c => c.Id.Equals(taskId)) ?? throw new KeyNotFoundException();
        TicketTask entity = _mapper.Map(model, target);
        await _taskRepository.UpdateAsync(entity);
    }

    public async Task UpdateStatus(int taskId, TicketTaskStatus newStatus)
    {
        var target = await _taskRepository.FirstOrDefaultAsync(c => c.Id.Equals(taskId)) ?? throw new KeyNotFoundException();
        target.TaskStatus = newStatus;
        await _taskRepository.UpdateAsync(target);
    }
}
