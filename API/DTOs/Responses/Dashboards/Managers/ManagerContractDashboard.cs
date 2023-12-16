namespace API.DTOs.Responses.Dashboards.Managers
{
    public class ManagerContractDashboard
    {
        public int PendingContractCount { get; set; }
        public int ActiveContractCount { get; set; }
        public int InActiveContractCount { get; set; }
        public int ExpiredContractCount { get; set; }
    }
}
