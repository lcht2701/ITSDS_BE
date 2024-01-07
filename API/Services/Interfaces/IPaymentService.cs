using API.DTOs.Requests.Payments;
using Domain.Models.Contracts;

namespace API.Services.Interfaces;

public interface IPaymentService
{
    Task<List<Payment>> Get();
    Task<List<Payment>> GetByContract(int contractId);
    Task<Payment> GetById(int id);
    Task<Payment> Create(CreatePaymentRequest model);
    Task<Payment> Update(int id, UpdatePaymentRequest model);
    Task Remove(int id);
    Task ClosePayment(int id);
}
