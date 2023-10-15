using API.DTOs.Requests.Categories;
using API.DTOs.Requests.TicketSolutions;
using API.DTOs.Requests.TicketTasks;
using API.DTOs.Responses.TicketTasks;
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

[Route("/v1/itsds/ticket/task")]
public class TicketTaskController : BaseController
{
    private readonly IRepositoryBase<TicketTask> _taskRepository;

    public TicketTaskController(IRepositoryBase<TicketTask> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet]
    public async Task<IActionResult> GetAllTasksOfTicket(int ticketId,
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var result = await _taskRepository.WhereAsync(x => x.TicketId.Equals(ticketId),
            new string[] { "Technician", "CreateBy", "Team", "Ticket" });
        var response = result.Select(task =>
        {
            var entity = Mapper.Map(task, new GetTicketTaskResponse());

            DataResponse.CleanNullableDateTime(entity);
            return entity;
        }).ToList();

        var pagedResponse = response.AsQueryable().GetPagedData(page, pageSize, filter, sort);

        return Ok(pagedResponse);
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPost("new")]
    public async Task<IActionResult> CreateTask(int ticketId, [FromBody] CreateTicketTaskRequest model)
    {
        var entity = Mapper.Map(model, new TicketTask());
        entity.TicketId = ticketId;
        entity.CreateById = CurrentUserID;
        await _taskRepository.CreateAsync(entity);
        return Ok("Create Successfully");
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTicketTask(int taskId, [FromBody] UpdateTicketTaskRequest req)
    {
        var target = await _taskRepository.FoundOrThrow(c => c.Id.Equals(taskId), new BadRequestException("Task not found")); 
        TicketTask entity = Mapper.Map(req, target);
        await _taskRepository.UpdateAsync(entity);
        return Ok("Update Successfully");
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTicketTask(int taskId)
    {
        var target = await _taskRepository.FoundOrThrow(c => c.Id.Equals(taskId), new BadRequestException("Task not found"));
        //Soft Delete
        await _taskRepository.DeleteAsync(target);
        return Ok("Delete Successfully");
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPatch("update-status")]
    public async Task<IActionResult> UpdateTaskStatus(int taskId, TicketTaskStatus newStatus)
    {
        var target = await _taskRepository.FoundOrThrow(c => c.Id.Equals(taskId), new BadRequestException("Task not found"));
        target.TaskStatus = newStatus;
        await _taskRepository.UpdateAsync(target);
        return Ok("Update Status Successfully");
    }
}
