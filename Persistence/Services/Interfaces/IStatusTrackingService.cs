using Domain.Constants.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services.Interfaces
{
    public interface IStatusTrackingService
    {
        Task<bool> UpdateTicketStatusTo(int ticketId, TicketStatus newStatus);
        Task<bool> UpdateTaskStatusTo(int taskId, TicketTaskStatus newStatus);
    }
}
