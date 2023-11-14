using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.Payments;

public class UpdatePaymentRequest : IMapTo<Payment>
{
    public string? Description { get; set; }

    public int NumberOfTerms { get; set; }

    public DateTime FirstDateOfPayment { get; set; }

    public int Duration { get; set; }

    public double InitialPaymentAmount { get; set; }

    public string? Note { get; set; }
}
