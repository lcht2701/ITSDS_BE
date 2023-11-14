namespace API.DTOs.Responses.Dashboards.Technicians
{
    public class TechnicianTicketDashboard
    {
        public int TotalTicket { get; set; }
        public int TotalAssignedTicket { get; set; }
        public int TotalInProgressTicket { get; set; }
        public int TotalResolvedTicket { get; set; }
        public int TotalCompletedTicket { get; set; }
    }
}
