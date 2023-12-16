namespace API.DTOs.Responses.Dashboards.Accountants
{
    public class AccountantDashboard
    {
        public int TotalContractCount { get; set; }
        public int ContractPaymentDoneCount { get; set; }
        public int ContractPaymentNotDoneCount { get; set; }
        public int ContractTermDoneCount { get; set; }
        public int ContractTermNotDoneCount { get; set; }
    }

    public class AccountantContractDashboard
    {
        public int PendingContractCount { get; set; }
        public int ActiveContractCount { get; set; }
        public int InActiveContractCount { get; set; }
        public int ExpiredContractCount { get; set; }
    }
}
