using API.DTOs.Requests.Assignments;
using API.DTOs.Responses.Assignments;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Cases;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class AssignmentService : IAssignmentService
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public AssignmentService(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IRepositoryBase<TeamMember> teamMemberRepository, IRepositoryBase<User> userRepository, ITicketService ticketService, IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _teamMemberRepository = teamMemberRepository;
        _userRepository = userRepository;
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public async Task<object> IsTechnicianMemberOfTeamAsync(int? technicianId, int? teamId)
    {
        var check = await _teamMemberRepository.FirstOrDefaultAsync(x =>
            x.MemberId.Equals(technicianId) && x.TeamId.Equals(teamId));

        return check;
    }

    public async Task<int> FindTechnicianWithLeastAssignments(int? teamId)
    {
        // Check if the teamId is provided; if not, return 0.
        if (!teamId.HasValue)
        {
            return 0;
        }

        // Retrieve assignments for the specified team.
        var assignments = await _assignmentRepository.WhereAsync(x => x.TeamId == teamId);

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

    public async Task<object> GetListOfTechnician(int? teamId)
    {
        List<User> users;
        if (teamId == null)
        {
            users = (List<User>)await _userRepository.WhereAsync(x => x.Role == Role.Technician);
        }
        else
        {
            var teamMembers = await _teamMemberRepository.WhereAsync(u => u.TeamId.Equals(teamId));
            var userIds = teamMembers.Select(tm => tm.MemberId).ToList();
            users = (List<User>)await _userRepository.WhereAsync(u => userIds.Contains(u.Id) && u.Role == Role.Technician);
        }

        var response = _mapper.Map<List<GetTechniciansResponse>>(users);
        return response;
    }

    public async Task<object> Get()
    {
        var entity = await _assignmentRepository.GetAsync(navigationProperties: new string[] { "Team", "Technician" });
        var response = _mapper.Map<List<GetAssignmentResponse>>(entity);
        return response;
    }

    public async Task<object> GetByTechnician(int technicianId)
    {
        var entity = await _assignmentRepository.WhereAsync(x => x.TechnicianId.Equals(technicianId), new string[] { "Team", "Technician" });
        var response = _mapper.Map<List<GetAssignmentResponse>>(entity);
        return response;
    }

    public async Task<object> GetByTeam(int teamId)
    {
        var entity = await _assignmentRepository.WhereAsync(x => x.TeamId.Equals(teamId), new string[] { "Team", "Technician" });
        var response = _mapper.Map<List<GetAssignmentResponse>>(entity);
        return response;
    }

    public async Task<object> GetById(int id)
    {
        var entity = await _assignmentRepository.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Team", "Technician" }) ?? throw new KeyNotFoundException();
        var response = _mapper.Map<GetAssignmentResponse>(entity);
        return response;
    }

    public async Task<IActionResult> Assign(int ticketId, AssignTicketManualRequest model)
    {

        var ticket = await _ticketRepository.FoundOrThrow(x => x.Id == ticketId, new KeyNotFoundException("Ticket not found."));

        var existingAssignment = await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId == ticketId);
        if (existingAssignment != null)
        {
            return new BadRequestObjectResult("Ticket has been assigned");
        }

        if (model.TechnicianId != null || model.TeamId != null)
        {
            if (model.TechnicianId != null && model.TeamId != null)
            {
                if (await IsTechnicianMemberOfTeamAsync(model.TechnicianId, model.TeamId) == null)
                {
                    return new BadRequestObjectResult("This technician is not a member of the specified team.");
                }
            }

            var assignment = new Assignment()
            {
                TicketId = ticketId,
                TechnicianId = model.TechnicianId,
                TeamId = model.TeamId
            };
            await _assignmentRepository.CreateAsync(assignment);
            await _ticketService.UpdateTicketStatus(ticketId, TicketStatus.Assigned);

            return new OkObjectResult("Assigned successfully");
        }
        else
        {
            return new OkResult();
        }
    }


    public async Task<IActionResult> Update(int ticketId, UpdateTicketAssignmentManualRequest model)
    {
        var ticket = await _ticketRepository.FoundOrThrow(x => x.Id == ticketId, new BadRequestException("Ticket Not Found"));
        var target = await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId.Equals(ticket.Id));

        if (model.TechnicianId != target.TechnicianId || model.TeamId != target.TeamId)
        {
            Assignment entity;

            switch (GetAssignmentCase(model.TechnicianId, model.TeamId))
            {
                case AssignmentCase.NullNull:
                    await _assignmentRepository.SoftDeleteAsync(target);
                    break;

                case AssignmentCase.NotNullNull:
                    entity = _mapper.Map(model, target);
                    entity.TeamId = null;
                    await _assignmentRepository.UpdateAsync(entity);
                    break;

                case AssignmentCase.NullNotNull:
                    entity = _mapper.Map(model, target);
                    entity.TechnicianId = null;
                    await _assignmentRepository.UpdateAsync(entity);
                    break;

                case AssignmentCase.NotNullNotNull:
                    if (await IsTechnicianMemberOfTeamAsync(model.TechnicianId, model.TeamId) == null)
                    {
                        return new BadRequestObjectResult("This technician is not a member of the specified team.");
                    }
                    entity = _mapper.Map(model, target);
                    await _assignmentRepository.UpdateAsync(entity);
                    break;

                default:
                    return new BadRequestObjectResult("Invalid modeluest.");
            }
        }
        return new OkObjectResult("Updated Successfully");
    }

    public async Task Remove(int ticketId)
    {
        var entity = await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId.Equals(ticketId)) ?? throw new KeyNotFoundException();
        await _assignmentRepository.SoftDeleteAsync(entity);
    }

    private AssignmentCase GetAssignmentCase(int? technicianId, int? teamId)
    {
        if (technicianId == null && teamId == null)
        {
            return AssignmentCase.NullNull;
        }
        else if (technicianId != null && teamId == null)
        {
            return AssignmentCase.NotNullNull;
        }
        else if (technicianId == null && teamId != null)
        {
            return AssignmentCase.NullNotNull;
        }
        else
        {
            return AssignmentCase.NotNullNotNull;
        }
    }
}
