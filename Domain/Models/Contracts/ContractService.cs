namespace Domain.Models.Contracts
{
    public partial class ContractService : BaseEntity
    {
        public ContractService()
        {
        }
        public int? ContractId { get; set; }

        public int? ServiceId { get; set; }

        public virtual Contract? Contract { get; set; }

        public virtual Service? Service { get; set; }
    }
}
