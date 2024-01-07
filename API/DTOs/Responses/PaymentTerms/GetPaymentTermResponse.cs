using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Responses.PaymentTerms
{
    public class GetPaymentTermResponse : IMapFrom<PaymentTerm>
    {
        public int Id { get; init; }

        public int PaymentId { get; set; }

        public double TermAmount { get; set; }

        public DateTime TermStart { get; set; }

        public DateTime TermEnd { get; set; }

        public bool IsPaid { get; set; }

        public DateTime? TermFinishTime { get; set; }

        public string? Note { get; set; }

        public List<string>? AttachmentUrls { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public virtual Payment? Payment { get; set; }
    }
}
