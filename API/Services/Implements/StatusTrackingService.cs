using API.Services.Interfaces;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Implements
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

        public bool isTicketDone(Ticket ticket)
        {
            return ticket.TicketStatus is TicketStatus.Closed or TicketStatus.Cancelled;
        }

        public async Task<bool> UpdateTicketStatusTo(Ticket ticket, TicketStatus newStatus)
        {
            try
            {
                ticket.TicketStatus = newStatus;
                await _ticketRepository.UpdateAsync(ticket);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
