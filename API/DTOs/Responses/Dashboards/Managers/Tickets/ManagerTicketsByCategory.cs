namespace API.DTOs.Responses.Dashboards.Managers.Tickets;

public class ManagerTicketsByCategory
{
    public List<TicketCategoryLine>? TicketCategoryLines { get; set; }
    public TicketCategoryTotal? Total { get; set; }
}
public class TicketCategoryLine
{
    public string? LineName { get; set; }

    public int OnGoingTicketsCount { get; set; }

    public int ClosedTicketsCount { get; set; }

    public int CancelledTicketsCount { get; set; }
}

public class TicketCategoryTotal
{

    public int TotalOnGoingTickets { get; set; }

    public int TotalClosedTicketsCount { get; set; }

    public int TotalCancelledTicketsCount { get; set; }
}
