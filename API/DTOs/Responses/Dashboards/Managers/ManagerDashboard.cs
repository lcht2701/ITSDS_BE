namespace API.DTOs.Responses.Dashboards.Managers
{
    public class ManagerDashboard
    {
        public int? CurrentResolvedTicketCount { get; set; }

        public int? AvailableContractsCount { get; set; }

        public int? TeamsCount { get; set; }

        public int? SolutionsCount { get; set; }
    }
}
