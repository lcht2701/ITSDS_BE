using API.Services.Interfaces;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class HangfireJobService : IHangfireJobService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IMessagingService _messagingService;
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Contract> _contractRepository;

    public HangfireJobService(IRepositoryBase<User> userRepository, IMessagingService messagingService, IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Contract> contractRepository)
    {
        _userRepository = userRepository;
        _messagingService = messagingService;
        _ticketRepository = ticketRepository;
        _contractRepository = contractRepository;
    }

    public async Task PeriodTicketSummaryNotificationJob()
    {
        if (DateTime.Today.DayOfWeek != DayOfWeek.Monday)
        {
            return;
        }
        
        var managerList = await _userRepository.WhereAsync(x => x.Role.Equals(Role.Manager));

        foreach (var manager in managerList)
        {
            var availableTicketsCount = (await _ticketRepository
                .WhereAsync(x => (x.TicketStatus != TicketStatus.Closed && x.TicketStatus != TicketStatus.Cancelled) &&
                                 x.IsPeriodic &&
                                 x.ScheduledStartTime >= DateTime.Today &&
                                 x.ScheduledStartTime <= DateTime.Today.AddDays(7)))
                .Count;

            if (availableTicketsCount > 0)
            {
                await _messagingService.SendNotification(
                    "Ticket Summary",
                    $"There are {availableTicketsCount} periodic tickets that need to be handled this week.",
                    manager.Id
                );
            }
        }
    }

    public async Task UpdateStatusOfContract()
    {
        var contracts = await _contractRepository.ToListAsync();
        foreach (var contract in contracts)
        {
            switch (contract.Status)
            {
                case ContractStatus.Pending:
                {
                    if (contract.StartDate >= DateTime.Now)
                    {
                        contract.Status = ContractStatus.Active;
                        await _contractRepository.UpdateAsync(contract);
                        foreach (var user in await _userRepository.WhereAsync(x => x.Role == Role.Manager || x.Role == Role.Accountant))
                        {
                            await _messagingService.SendNotification("Contract Update", $"Contract {contract.Description} has been updated to Active.", user.Id);
                        }
                    }
                    break;
                }
                case ContractStatus.Active:
                case ContractStatus.Inactive:
                {
                    if (contract.EndDate >= DateTime.Now)
                    {
                        contract.Status = ContractStatus.Expired;
                        await _contractRepository.UpdateAsync(contract);
                        foreach (var user in await _userRepository.WhereAsync(x => x.Role == Role.Manager || x.Role == Role.Accountant))
                        {
                            await _messagingService.SendNotification("Contract Update", $"Contract {contract.Description} has been updated to Expired.", user.Id);
                        }
                    }
                    break;
                }
            }

            
        }
    }
}