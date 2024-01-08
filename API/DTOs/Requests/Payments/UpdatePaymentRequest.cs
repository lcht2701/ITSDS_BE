using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.Payments;

public class UpdatePaymentRequest : IMapTo<Payment>
{
    public bool IsFullyPaid { get; set; }

    public string? Note { get; set; }

    public List<string>? AttachmentUrls { get; set; }
}
