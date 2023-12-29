namespace API.DTOs.Responses.Dashboards.Managers
{
    public class ManagerDashboard
    {
        public int? TotalContractOfDay { get; set; }

        public int? TotalTicketOfDay { get; set; }

        public int? TotalSolutionOfDay { get; set; }

        public double? TotalPaymentOfDay { get; set; }
    }
}
