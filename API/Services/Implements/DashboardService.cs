using API.DTOs.Responses.Dashboards.Customers;
using API.DTOs.Responses.Dashboards.Managers.Tickets;
using API.DTOs.Responses.Dashboards.Technicians;
using API.Services.Interfaces;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using System.Globalization;

namespace API.Services.Implements;

public class DashboardService : IDashboardService
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IRepositoryBase<Category> _categoryRepository;
    private readonly IRepositoryBase<Mode> _modeRepository;
    private readonly IRepositoryBase<Service> _serviceRepository;

    public DashboardService(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository, IRepositoryBase<Category> categoryRepository, IRepositoryBase<Mode> modeRepository, IRepositoryBase<Service> serviceRepository)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _categoryRepository = categoryRepository;
        _modeRepository = modeRepository;
        _serviceRepository = serviceRepository;
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

    public async Task<ManagerTicketsDashboardTable> GetManagerTicketsByCategory()
    {
        var list = await _categoryRepository.ToListAsync();
        List<DashboardTableRow> rows = new();
        foreach (var item in list)
        {
            rows.Add(new DashboardTableRow
            {
                LineName = item.Name!,
                OnGoingTicketsCount = (await _ticketRepository.WhereAsync(x => x.CategoryId == item.Id && (
                                                                            x.TicketStatus.Equals(TicketStatus.Assigned) ||
                                                                            x.TicketStatus.Equals(TicketStatus.InProgress) ||
                                                                            x.TicketStatus.Equals(TicketStatus.Resolved)
                                                                            ))).Count,
                ClosedTicketsCount = (await _ticketRepository.WhereAsync(x => x.CategoryId == item.Id && x.TicketStatus.Equals(TicketStatus.Closed))).Count,
                CancelledTicketsCount = (await _ticketRepository.WhereAsync(x => x.CategoryId == item.Id && x.TicketStatus.Equals(TicketStatus.Cancelled))).Count,

            });
        }

        DashboardTableTotal total = new()
        {
            TotalOnGoingTickets = rows.Sum(x => x.OnGoingTicketsCount),
            TotalClosedTicketsCount = rows.Sum(x => x.ClosedTicketsCount),
            TotalCancelledTicketsCount = rows.Sum(x => x.CancelledTicketsCount),
        };

        return new ManagerTicketsDashboardTable
        {
            Rows = rows.ToList(),
            Total = total,
        };
    }

    public async Task<ManagerTicketsDashboardTable> GetManagerTicketsByPriority()
    {
        var list = Enum.GetValues(typeof(Priority)).Cast<Priority>().ToList(); ;
        List<DashboardTableRow> rows = new();
        foreach (var item in list)
        {
            rows.Add(new DashboardTableRow
            {
                LineName = DataResponse.GetEnumDescription(item),
                OnGoingTicketsCount = (await _ticketRepository.WhereAsync(x => x.Priority == item && (
                                                                            x.TicketStatus.Equals(TicketStatus.Assigned) ||
                                                                            x.TicketStatus.Equals(TicketStatus.InProgress) ||
                                                                            x.TicketStatus.Equals(TicketStatus.Resolved)
                                                                            ))).Count,
                ClosedTicketsCount = (await _ticketRepository.WhereAsync(x => x.Priority == item && x.TicketStatus.Equals(TicketStatus.Closed))).Count,
                CancelledTicketsCount = (await _ticketRepository.WhereAsync(x => x.Priority == item && x.TicketStatus.Equals(TicketStatus.Cancelled))).Count,
            });
        }

        DashboardTableTotal total = new()
        {
            TotalOnGoingTickets = rows.Sum(x => x.OnGoingTicketsCount),
            TotalClosedTicketsCount = rows.Sum(x => x.ClosedTicketsCount),
            TotalCancelledTicketsCount = rows.Sum(x => x.CancelledTicketsCount),
        };

        return new ManagerTicketsDashboardTable
        {
            Rows = rows.ToList(),
            Total = total,
        };
    }

    public async Task<ManagerTicketsDashboardTable> GetManagerTicketsByMode()
    {
        var list = await _modeRepository.ToListAsync();
        List<DashboardTableRow> rows = new();
        foreach (var item in list)
        {
            rows.Add(new DashboardTableRow
            {
                LineName = item.Name!,
                OnGoingTicketsCount = (await _ticketRepository.WhereAsync(x => x.ModeId == item.Id && (
                                                                            x.TicketStatus.Equals(TicketStatus.Assigned) ||
                                                                            x.TicketStatus.Equals(TicketStatus.InProgress) ||
                                                                            x.TicketStatus.Equals(TicketStatus.Resolved)
                                                                            ))).Count,
                ClosedTicketsCount = (await _ticketRepository.WhereAsync(x => x.ModeId == item.Id && x.TicketStatus.Equals(TicketStatus.Closed))).Count,
                CancelledTicketsCount = (await _ticketRepository.WhereAsync(x => x.ModeId == item.Id && x.TicketStatus.Equals(TicketStatus.Cancelled))).Count,

            });
        }

        DashboardTableTotal total = new()
        {
            TotalOnGoingTickets = rows.Sum(x => x.OnGoingTicketsCount),
            TotalClosedTicketsCount = rows.Sum(x => x.ClosedTicketsCount),
            TotalCancelledTicketsCount = rows.Sum(x => x.CancelledTicketsCount),
        };

        return new ManagerTicketsDashboardTable
        {
            Rows = rows.ToList(),
            Total = total,
        };
    }

    public async Task<ManagerTicketsDashboardTable> GetManagerTicketsByService()
    {
        var list = await _serviceRepository.ToListAsync();
        List<DashboardTableRow> rows = new();
        foreach (var item in list)
        {
            rows.Add(new DashboardTableRow
            {
                LineName = item.Description!,
                OnGoingTicketsCount = (await _ticketRepository.WhereAsync(x => x.ServiceId == item.Id && (
                                                                            x.TicketStatus.Equals(TicketStatus.Assigned) ||
                                                                            x.TicketStatus.Equals(TicketStatus.InProgress) ||
                                                                            x.TicketStatus.Equals(TicketStatus.Resolved)
                                                                            ))).Count,
                ClosedTicketsCount = (await _ticketRepository.WhereAsync(x => x.ServiceId == item.Id && x.TicketStatus.Equals(TicketStatus.Closed))).Count,
                CancelledTicketsCount = (await _ticketRepository.WhereAsync(x => x.ServiceId == item.Id && x.TicketStatus.Equals(TicketStatus.Cancelled))).Count,

            });
        }

        DashboardTableTotal total = new()
        {
            TotalOnGoingTickets = rows.Sum(x => x.OnGoingTicketsCount),
            TotalClosedTicketsCount = rows.Sum(x => x.ClosedTicketsCount),
            TotalCancelledTicketsCount = rows.Sum(x => x.CancelledTicketsCount),
        };

        return new ManagerTicketsDashboardTable
        {
            Rows = rows.ToList(),
            Total = total,
        };
    }

    public async Task<List<DashboardTableRow>> GetTicketDashboardByWeek(DateTime currentDate)
    {
        DayOfWeek currentDayOfWeek = currentDate.DayOfWeek;
        int daysUntilMonday = ((int)DayOfWeek.Tuesday - (int)currentDayOfWeek - 7) % 7;
        DateTime startOfLastWeek = currentDate.AddDays(daysUntilMonday).Date;
        DateTime endOfLastWeek = startOfLastWeek.AddDays(6);

        var result = (await _ticketRepository
            .WhereAsync(x => x.CreatedAt >= startOfLastWeek && x.CreatedAt <= endOfLastWeek))
            .GroupBy(x => x.CreatedAt?.DayOfWeek)
            .OrderBy(group => group.Key)  // Order by DayOfWeek with Monday first
            .Select(group => new DashboardTableRow
            {
                LineName = group.Key.ToString(),
                OnGoingTicketsCount = group.Count(x =>
                    x.TicketStatus == TicketStatus.Assigned ||
                    x.TicketStatus == TicketStatus.InProgress ||
                    x.TicketStatus == TicketStatus.Resolved),
                ClosedTicketsCount = group.Count(x => x.TicketStatus == TicketStatus.Closed),
                CancelledTicketsCount = group.Count(x => x.TicketStatus == TicketStatus.Cancelled)
            })
            .ToList();

        var allDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
        var resultWithAllDays = allDays
            .Select(day => result.FirstOrDefault(r => r.LineName == day.ToString()) ?? new DashboardTableRow
            {
                LineName = day.ToString(),
                OnGoingTicketsCount = 0,
                ClosedTicketsCount = 0,
                CancelledTicketsCount = 0
            })
            .ToList();

        return resultWithAllDays;
    }

    public async Task<List<DashboardTableRow>> GetTicketDashboardByMonth(DateTime currentDate)
    {
        DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var allTicketsInMonth = (await _ticketRepository
             .WhereAsync(t => t.CreatedAt >= startOfMonth && t.CreatedAt <= endOfMonth))
             .ToList();

        var ticketTotalsByWeek = allTicketsInMonth
            .GroupBy(t => (t.CreatedAt.Value - startOfMonth).Days / 7 + 1)
            .Select(group => new DashboardTableRow
            {
                LineName = $"Week {group.Key}",
                OnGoingTicketsCount = group.Count(t => t.TicketStatus.Equals(TicketStatus.Assigned) ||
                                                        t.TicketStatus.Equals(TicketStatus.InProgress) ||
                                                        t.TicketStatus.Equals(TicketStatus.Resolved)),
                ClosedTicketsCount = group.Count(t => t.TicketStatus == TicketStatus.Closed),
                CancelledTicketsCount = group.Count(t => t.TicketStatus == TicketStatus.Cancelled)
            })
            .OrderBy(group => group.LineName)  // Order by week name
            .ToList();

        // Ensure all 5 weeks are included
        var allWeeks = Enumerable.Range(1, 5);
        var resultWithAllWeeks = allWeeks
            .Select(week => ticketTotalsByWeek.FirstOrDefault(r => r.LineName == $"Week {week}") ?? new DashboardTableRow
            {
                LineName = $"Week {week}",
                OnGoingTicketsCount = 0,
                ClosedTicketsCount = 0,
                CancelledTicketsCount = 0
            })
            .ToList();

        return resultWithAllWeeks;
    }
}
