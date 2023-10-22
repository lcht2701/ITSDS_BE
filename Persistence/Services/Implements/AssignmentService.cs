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
        // Check if the teamId is provided; if not, return 0.
        if (!teamId.HasValue)
        {
            return 0;
        }

        // Retrieve assignments for the specified team.
        var assignments = await _assignmentRepo.WhereAsync(x => x.TeamId == teamId);

        // If there are no assignments for the team, return 0.
        if (!assignments.Any())
        {
            return 0;
        }

        // Create a dictionary to store technician assignment counts.
        var assignmentCounts = new Dictionary<int, int>();

        foreach (var assignment in assignments)
        {
            // If the technicianId is null, skip this assignment.
            if (!assignment.TechnicianId.HasValue)
            {
                continue;
            }

            int technicianId = assignment.TechnicianId.Value;

            // Update or initialize the assignment count for the technician.
            if (assignmentCounts.ContainsKey(technicianId))
            {
                assignmentCounts[technicianId]++;
            }
            else
            {
                assignmentCounts[technicianId] = 1;
            }
        }

        // Find the minimum assignment count.
        int minAssignments = assignmentCounts.Min(kv => kv.Value);

        // Get the technicians with the minimum assignment count.
        var techniciansWithMinAssignments = assignmentCounts
            .Where(kv => kv.Value == minAssignments)
            .Select(kv => kv.Key)
            .ToList();

        // If there's only one technician with the minimum assignments, return that technician.
        if (techniciansWithMinAssignments.Count == 1)
        {
            return techniciansWithMinAssignments.First();
        }
        else
        {
            // Randomly select one technician if there are multiple with the same minimum assignments.
            Random random = new Random();
            int randomIndex = random.Next(techniciansWithMinAssignments.Count);
            return techniciansWithMinAssignments[randomIndex];
        }
    }

}
