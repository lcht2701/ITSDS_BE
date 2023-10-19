using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;
using Persistence.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services.Implements;

public class AssignmentService : IAssignmentService
{
    private readonly IRepositoryBase<Assignment> _assignmentRepo;

    public AssignmentService(IRepositoryBase<Assignment> assignmentRepo)
    {
        _assignmentRepo = assignmentRepo;
    }

    public async Task<int> FindTechnicianWithLeastAssignments(int? teamId)
    {
        var assignments = await _assignmentRepo.WhereAsync(x => x.TeamId.Equals(teamId));
        var technicianIds = assignments.Select(x => x.TechnicianId).Distinct().ToList();

        if (!technicianIds.Any())
        {
            return 0;
        }
        else
        {
            int minAssignments = int.MaxValue;
            int technicianWithLeastAssignmentsId = 0;

            foreach (var technicianId in technicianIds)
            {
                int assignmentsCount = assignments.Count(x => x.TechnicianId == technicianId);

                if (assignmentsCount < minAssignments)
                {
                    minAssignments = assignmentsCount;
                    technicianWithLeastAssignmentsId = (int)technicianId;
                }
            }

            return technicianWithLeastAssignmentsId;
        }
    }

}
