﻿using API.DTOs.Requests.TicketTasks;
using API.DTOs.Responses.TicketTasks;
using Domain.Constants.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Services.Interfaces
{
    public interface ITicketTaskService
    {
        Task<List<GetTicketTaskResponse>> Get(int ticketId);
        Task Create(int ticketId, CreateTicketTaskRequest model, int createdBy);
        Task Update(int taskId, UpdateTicketTaskRequest model);
        Task Remove(int taskId);
        Task UpdateTaskStatus(int taskId, TicketTaskStatus newStatus);
    }
}
