using Domain.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task<int> FindTechnicianWithLeastAssignments(int? teamId);
    }
}
