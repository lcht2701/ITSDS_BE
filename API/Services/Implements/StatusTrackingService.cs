using API.Services.Interfaces;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

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

    public async Task UpdateTicketStatus(int ticketId, TicketStatus newStatus)
    {

        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId) ?? throw new KeyNotFoundException();
        switch (ticket.TicketStatus)
        {
            case TicketStatus.Open:
            case TicketStatus.Assigned:
                if (ticket.TicketStatus != TicketStatus.Closed)
                {
                    ticket.TicketStatus = newStatus;
                    await _ticketRepository.UpdateAsync(ticket);
                }
                else
                {
                    throw new BadRequestException("Ticket Status cannot update to Closed immediately");
                }
                break;
            case TicketStatus.InProgress:
                if (ticket.TicketStatus != TicketStatus.Closed)
                {
                    ticket.TicketStatus = newStatus;
                    await _ticketRepository.UpdateAsync(ticket);
                }
                else
                {
                    throw new BadRequestException();
                }
                break;
            case TicketStatus.Resolved:
                break;
            case TicketStatus.Closed:
                break;
            case TicketStatus.Cancelled:
                if (ticket.TicketStatus == TicketStatus.Open)
                {
                    ticket.TicketStatus = newStatus;
                    await _ticketRepository.UpdateAsync(ticket);
                }
                else
                {
                    throw new BadRequestException("Ticket cannot be cancelled after it being assigned");
                }
                break;
        }
    }
}
