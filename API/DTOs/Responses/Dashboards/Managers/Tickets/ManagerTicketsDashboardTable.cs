namespace API.DTOs.Responses.Dashboards.Managers.Tickets;

public class ManagerTicketsDashboardTable
{
    public List<DashboardTableRow>? Rows { get; set; }
    public DashboardTableTotal? Total { get; set; }
}
public class DashboardTableRow
{
    public string? LineName { get; set; }

    public int OnGoingTicketsCount { get; set; }

    public int ClosedTicketsCount { get; set; }

    public int CancelledTicketsCount { get; set; }
}

public class DashboardTableTotal
{

    public int TotalOnGoingTickets { get; set; }

    public int TotalClosedTicketsCount { get; set; }

    public int TotalCancelledTicketsCount { get; set; }
}
