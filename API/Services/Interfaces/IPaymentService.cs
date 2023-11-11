using API.DTOs.Requests.Payments;
using Domain.Models.Contracts;

namespace API.Services.Interfaces;

public interface IPaymentService
{
    //Payment
    Task<List<Payment>> Get();
    Task<Payment> GetByContract(int contractId);
    Task<Payment> GetById(int id);
    Task<Payment> Create(CreatePaymentRequest model);
    Task<Payment> Update(int id, UpdatePaymentRequest model);
    Task Remove(int id);
    //Payment Terms
    Task<List<PaymentTerm>> GetPaymentTerms(int paymentId);
    Task<List<PaymentTerm>> UpdatePaymentTerm(int paymentId);
    Task<List<PaymentTerm>> RemovePaymentTerm(int paymentId);
}
