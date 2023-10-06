using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task<bool> AssignTechnicianToTicket(int ticketId, int technicianId);
        Task<bool> UpdateTechnicianAssignment(int ticketId, int newTechnicianId);

    }
}
