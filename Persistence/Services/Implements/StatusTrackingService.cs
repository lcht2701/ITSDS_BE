using Domain.Constants.Enums;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;
using Persistence.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services.Implements
{
    public class StatusTrackingService : IStatusTrackingService
    {
        private readonly IRepositoryBase<Ticket> _ticketRepository;
        private readonly IRepositoryBase<TicketTask> _taskRepository;

        public StatusTrackingService(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<TicketTask> taskRepository)
        {
            _ticketRepository = ticketRepository;
            _taskRepository = taskRepository;
        }

        public async Task<bool> UpdateTicketStatusTo(int ticketId, TicketStatus newStatus)
        {
            try
            {
                var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(ticketId));
                ticket.TicketStatus = newStatus;
                await _ticketRepository.UpdateAsync(ticket);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateTaskStatusTo(int taskId, TicketTaskStatus newStatus)
        {
            try
            {
                var target = await _taskRepository.FirstOrDefaultAsync(x => x.Id.Equals(taskId));
                target.TaskStatus = newStatus;
                await _taskRepository.UpdateAsync(target);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
