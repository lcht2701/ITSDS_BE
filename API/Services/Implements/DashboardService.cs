using API.DTOs.Responses.Dashboards.Customers;
using API.DTOs.Responses.Dashboards.Managers.Tickets;
using API.DTOs.Responses.Dashboards.Technicians;
using API.Services.Interfaces;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class DashboardService : IDashboardService
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IRepositoryBase<Category> _categoryRepository;

    public DashboardService(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IRepositoryBase<Category> categoryRepository)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<CustomerTicketDashboard> GetCustomerTicketDashboard(int userId)
    {
        CustomerTicketDashboard model = new();
        try
        {
            model.TotalTicket = (await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId))).Count;
            model.TotalOpenTicket = (await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId) && x.TicketStatus.Equals(TicketStatus.Open))).Count;
            model.TotalAssignedTicket = (await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId) && x.TicketStatus.Equals(TicketStatus.Assigned))).Count;
            model.TotalInProgressTicket = (await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId) && x.TicketStatus.Equals(TicketStatus.InProgress))).Count;
            model.TotalResolvedTicket = (await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId) && x.TicketStatus.Equals(TicketStatus.Resolved))).Count;
            model.TotalClosedTicket = (await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId) && x.TicketStatus.Equals(TicketStatus.Closed))).Count;
            model.TotalCancelledTicket = (await _ticketRepository.WhereAsync(x => x.RequesterId.Equals(userId) && x.TicketStatus.Equals(TicketStatus.Cancelled))).Count;
        }
        catch (Exception ex)
        {
            throw new ServerFailureException(ex.Message);
        }
        return model;
    }

    public async Task<TechnicianTicketDashboard> GetTechnicianTicketDashboard(int userId)
    {
        TechnicianTicketDashboard model = new();
        try
        {
            var ticketIds = (await _assignmentRepository.WhereAsync(x => x.TechnicianId.Equals(userId))).Select(x => x.TicketId);

            model.TotalTicket = (await _ticketRepository.WhereAsync(x => ticketIds.Contains(x.Id))).Count;
            model.TotalAssignedTicket = (await _ticketRepository.WhereAsync(x => ticketIds.Contains(x.Id) && x.TicketStatus.Equals(TicketStatus.Assigned))).Count;
            model.TotalInProgressTicket = (await _ticketRepository.WhereAsync(x => ticketIds.Contains(x.Id) && x.TicketStatus.Equals(TicketStatus.InProgress))).Count;
            model.TotalResolvedTicket = (await _ticketRepository.WhereAsync(x => ticketIds.Contains(x.Id) && x.TicketStatus.Equals(TicketStatus.Resolved))).Count;
            model.TotalCompletedTicket = (await _ticketRepository.WhereAsync(x => ticketIds.Contains(x.Id) && x.TicketStatus.Equals(TicketStatus.Closed))).Count;
        }
        catch (Exception ex)
        {
            throw new ServerFailureException(ex.Message);
        }
        return model;
    }

    public async Task<ManagerTicketDashboard> GetManagerTicketDashboard()
    {
        ManagerTicketDashboard model = new();
        try
        {
            model.TotalTicket = (await _ticketRepository.ToListAsync()).Count;
            model.TotalOpenTicket = (await _ticketRepository.WhereAsync(x => x.TicketStatus.Equals(TicketStatus.Open))).Count;
            model.TotalAssignedTicket = (await _ticketRepository.WhereAsync(x => x.TicketStatus.Equals(TicketStatus.Assigned))).Count;
            model.TotalInProgressTicket = (await _ticketRepository.WhereAsync(x => x.TicketStatus.Equals(TicketStatus.InProgress))).Count;
            model.TotalResolvedTicket = (await _ticketRepository.WhereAsync(x => x.TicketStatus.Equals(TicketStatus.Resolved))).Count;
            model.TotalClosedTicket = (await _ticketRepository.WhereAsync(x => x.TicketStatus.Equals(TicketStatus.Closed))).Count;
            model.TotalCancelledTicket = (await _ticketRepository.WhereAsync(x => x.TicketStatus.Equals(TicketStatus.Cancelled))).Count;
        }
        catch (Exception ex)
        {
            throw new ServerFailureException(ex.Message);
        }
        return model;
    }

    public async Task<ManagerTicketsByCategory> GetManagerTicketsByCategory()
    {
        ManagerTicketsByCategory data = new();
        var categoryList = await _categoryRepository.ToListAsync();

        List<TicketCategoryLine> lines = new();
        foreach (var category in categoryList)
        {
            lines.Add(new TicketCategoryLine
            {
                LineName = category.Name!,
                OnGoingTicketsCount = (await _ticketRepository.WhereAsync(x => x.CategoryId == category.Id
                                                                            && (x.TicketStatus.Equals(TicketStatus.Assigned)
                                                                                || x.TicketStatus.Equals(TicketStatus.InProgress)
                                                                                || x.TicketStatus.Equals(TicketStatus.Resolved))
                                                                            )).Count,
                ClosedTicketsCount = (await _ticketRepository.WhereAsync(x => x.CategoryId == category.Id && x.TicketStatus.Equals(TicketStatus.Closed))).Count,
                CancelledTicketsCount = (await _ticketRepository.WhereAsync(x => x.CategoryId == category.Id && x.TicketStatus.Equals(TicketStatus.Cancelled))).Count,

            });
        }

        TicketCategoryTotal total = new();

        return data;
    }
}
