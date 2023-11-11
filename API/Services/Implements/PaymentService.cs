using API.DTOs.Requests.Payments;
using API.Services.Interfaces;
using Domain.Models.Contracts;

namespace API.Services.Implements;

public class PaymentService : IPaymentService
{
    public Task<Payment> Create(CreatePaymentRequest model)
    {
        throw new NotImplementedException();
    }

    public Task<List<Payment>> Get()
    {
        throw new NotImplementedException();
    }

    public Task<Payment> GetByContract(int contractId)
    {
        throw new NotImplementedException();
    }

    public Task<Payment> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<PaymentTerm>> GetPaymentTerms(int paymentId)
    {
        throw new NotImplementedException();
    }

    public Task Remove(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<PaymentTerm>> RemovePaymentTerm(int paymentId)
    {
        throw new NotImplementedException();
    }

    public Task<Payment> Update(int id, UpdatePaymentRequest model)
    {
        throw new NotImplementedException();
    }

    public Task<List<PaymentTerm>> UpdatePaymentTerm(int paymentId)
    {
        throw new NotImplementedException();
    }
}
