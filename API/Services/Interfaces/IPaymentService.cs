using API.DTOs.Requests.Payments;
using API.DTOs.Requests.PaymentTerms;
using Domain.Models.Contracts;

namespace API.Services.Interfaces;

public interface IPaymentService
{
    //Payment
    Task<List<Payment>> Get();
    Task<List<Payment>> GetByContract(int contractId);
    Task<Payment> GetById(int id);
    Task<Payment> Create(CreatePaymentRequest model);
    Task<Payment> Update(int id, UpdatePaymentRequest model);
    Task Remove(int id);
    Task ClosePayment(int id);
    //Payment Terms
    Task<List<PaymentTerm>> GetPaymentTerms(int paymentId);
    Task<List<PaymentTerm>> GeneratePaymentTerms(int paymentId);
    Task<PaymentTerm> UpdatePaymentTerm(int paymentId, UpdatePaymentTermRequest model);
    Task RemovePaymentTerm(int paymentId);
    Task SendPaymentNotification(int termId);
}
