using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.Payments
{
    public class CreatePaymentRequest : IMapTo<Payment>
    {
        public int ContractId { get; set; }

        public string? Description { get; set; }

        public DateTime StartDateOfPayment { get; set; }

        public int DaysAmountForPayment { get; set; }

        public string? Note { get; set; }

        public List<string>? AttachmentUrls { get; set; }
    }
}

