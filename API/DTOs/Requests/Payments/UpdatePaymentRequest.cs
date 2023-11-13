using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.Payments;

public class UpdatePaymentRequest : IMapTo<Payment>
{
    public string? Description { get; set; }

    public string? PaymentType { get; set; }

    public bool? IsMultiplePayment { get; set; }

    public DateTime? PaymentStart { get; set; }

    public DateTime? PaymentEnd { get; set; }

    public bool? IsFullyPaid { get; set; }

    public string? Note { get; set; }
}
