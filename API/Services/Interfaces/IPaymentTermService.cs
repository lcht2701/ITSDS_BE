using API.DTOs.Requests.PaymentTerms;
using API.DTOs.Responses.PaymentTerms;
using Domain.Models.Contracts;

namespace API.Services.Interfaces;

public interface IPaymentTermService
{
    Task<List<GetPaymentTermResponse>> GetPaymentTerms(int paymentId);
    Task<GetPaymentTermResponse> GetPaymentTermById(int termId);
    Task<List<PaymentTerm>> GeneratePaymentTerms(int paymentId);
    Task<PaymentTerm> UpdatePaymentTerm(int paymentId, UpdatePaymentTermRequest model);
    Task RemovePaymentTerm(int paymentId);
}
