﻿using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services.Interfaces
{
    public interface IStatusTrackingService
    {
        bool isTicketDone(Ticket ticket);
        Task<bool> UpdateTicketStatusTo(Ticket ticket, TicketStatus newStatus);
    }
}
