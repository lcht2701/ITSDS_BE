namespace Domain.Models.Contracts
{
    public partial class Renewal : BaseEntity
    {
        public Renewal() { }

        public int? ContractId { get; set; }

        public string? Description { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public double? Value { get; set; }

        public DateTime? RenewedDate { get; set; }

        public int? RenewedById { get; set; }

        public virtual Contract? Contract { get; set; }

        public virtual User? RenewedBy { get; set; }
    }
}
