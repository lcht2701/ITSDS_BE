using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.Payments
{
    public class CreatePaymentRequest : IMapTo<Payment>
    {
        public int? ContractId { get; set; }

        public string? Description { get; set; }

        public string? PaymentType { get; set; }

        public bool? IsMultiplePayment { get; set; }

        public DateTime? PaymentStart { get; set; }

        public DateTime? PaymentEnd { get; set; }
    }
}
