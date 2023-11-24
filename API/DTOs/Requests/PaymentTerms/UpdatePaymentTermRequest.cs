using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.PaymentTerms
{
    public class UpdatePaymentTermRequest : IMapTo<PaymentTerm>
    {
        public bool? IsPaid { get; set; }

        public string? Note { get; set; }

        public string? AttachmentUrl { get; set; }
    }
}
