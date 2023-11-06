﻿namespace API.DTOs.Responses.Dashboards.Tickets
{
    public class CustomerTicketModel
    {
        public int TotalTicket { get; set; }
        public int TotalOpenTicket { get; set; }
        public int TotalAssignedTicket { get; set; }
        public int TotalInProgressTicket { get; set; }
        public int TotalResolvedTicket { get; set; }
        public int TotalClosedTicket { get; set; }
        public int TotalCancelledTicket { get; set; }
    }
}
