using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IStatusTrackingService
    {
        bool isTicketDone(Ticket ticket);
        Task UpdateTicketStatus(int ticketId, TicketStatus newStatus);
    }
}
