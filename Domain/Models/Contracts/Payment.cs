using System.Text.Json.Serialization;

namespace Domain.Models.Contracts
{
    public partial class Payment : BaseEntity
    {
        public Payment()
        {
            PaymentTerms = new HashSet<PaymentTerm>();
        }

        public string Description { get; set; }

        public string? PaymentType { get; set; }

        public DateTime PaymentStart { get; set; }

        public DateTime PaymentEnd { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime? PaymentFinishTime { get; set; }

        public string? Note { get; set; }

        public int ContractId { get; set; }

        public virtual Contract Contract { get; set; }

        [JsonIgnore]
        public virtual ICollection<PaymentTerm>? PaymentTerms { get; set; }
    }
}
