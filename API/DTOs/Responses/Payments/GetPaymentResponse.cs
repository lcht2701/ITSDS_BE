using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Responses.Payments
{
    public class GetPaymentResponse : IMapFrom<Payment>
    {
        public int Id { get; init; }

        public int ContractId { get; set; }

        public string? Description { get; set; }

        public DateTime StartDateOfPayment { get; set; }

        public DateTime EndDateOfPayment { get; set; }

        public bool IsFullyPaid { get; set; }

        public DateTime? PaymentFinishTime { get; set; }

        public string? Note { get; set; }

        public List<string>? AttachmentUrls { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public virtual Contract? Contract { get; set; }
    }
}
