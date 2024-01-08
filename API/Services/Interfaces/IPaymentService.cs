using API.DTOs.Requests.Payments;
using API.DTOs.Responses.Payments;
using Domain.Models.Contracts;

namespace API.Services.Interfaces;

public interface IPaymentService
{
    Task<List<GetPaymentResponse>> Get();
    Task<GetPaymentResponse> GetByContract(int contractId);
    Task<GetPaymentResponse> GetById(int id);
    Task<Payment> Create(CreatePaymentRequest model);
    Task<Payment> Update(int id, UpdatePaymentRequest model);
    Task Remove(int id);
}
