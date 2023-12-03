namespace Domain.Models.Contracts
{
    public partial class ServiceContract : BaseEntity
    {
        public ServiceContract()
        {
        }

        public int? ServiceId { get; set; }

        public int? ContractId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? Frequency {  get; set; } 

        public virtual Contract? Contract { get; set; }

        public virtual Service? Service { get; set; }
    }
}
