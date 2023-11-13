namespace Domain.Models.Contracts
{
    public partial class PaymentTerm : BaseEntity
    {
        public PaymentTerm()
        {

        }

        public int? PaymentId { get; set; }

        public string? Description { get; set; }

        public double? TermAmount { get; set; }

        public DateTime? TermStart { get; set; }

        public DateTime? TermEnd { get; set; }

        public bool? IsPaid { get; set; }

        public DateTime? TermFinishTime { get; set; }

        public string? Note { get; set; }

        public virtual Payment? Payment { get; set; }
    }
}
