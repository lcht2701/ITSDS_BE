namespace Domain.Models.Contracts
{
    public partial class PaymentTerm : BaseEntity
    {
        public PaymentTerm()
        {

        }

        public int? PaymentId { get; set; }

        public virtual Payment? Payment { get; set; }
    }
}
