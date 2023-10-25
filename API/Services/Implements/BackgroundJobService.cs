using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Tickets;
using Hangfire;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public BackgroundJobService(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IRepositoryBase<TeamMember> teamMemberRepository, IRepositoryBase<User> userRepository, ITicketService ticketService, IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _teamMemberRepository = teamMemberRepository;
        _userRepository = userRepository;
        _ticketService = ticketService;
        _mapper = mapper;
    }
    public async Task AssignSupportJob(int ticketId)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
        {
            // Handle the case where the ticket does not exist
            return;
        }

        var availableTechnicians = await _userRepository.WhereAsync(x => x.Role == Role.Technician && x.IsActive == true);

        if (!availableTechnicians.Any())
        {
            // Handle the case where no technicians are available
            return;
        }

        int selectedTechnician = -1;
        int minValue = int.MaxValue;

        foreach (var technician in availableTechnicians)
        {
            int count = await GetNumberOfAssignmentsForTechnician(technician.Id);
            if (count < minValue)
            {
                minValue = count;
                selectedTechnician = technician.Id;
            }
        }

        if (selectedTechnician != -1)
        {
            var assignment = new Assignment()
            {
                TicketId = ticketId,
                TechnicianId = selectedTechnician
            };

            await _assignmentRepository.CreateAsync(assignment);
            await _ticketService.UpdateTicketStatus(ticketId, TicketStatus.Assigned);
        }
        else
        {
            // Handle the case where no technician is available
            // You might want to log this or take other actions
        }
    }

    public async Task CancelAssignSupportJob(string jobId, int ticketId)
    {
        if (await _ticketService.IsTicketAssigned(ticketId) == true)
        {
            BackgroundJob.Delete(jobId);
        }
    }

    public async Task CloseTicketJob(int ticketId)
    {
        await _ticketService.UpdateTicketStatus(ticketId, TicketStatus.Closed);
    }

    public async Task CancelCloseTicketJob(string jobId, int ticketId)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(ticketId));
        if (ticket.TicketStatus == TicketStatus.Closed)
        {
            BackgroundJob.Delete(jobId);
        }

    }

    private async Task<int> GetNumberOfAssignmentsForTechnician(int technicianId)
    {
        var result = await _assignmentRepository.WhereAsync(x => x.TechnicianId == technicianId);
        return result.Count;
    }
}
