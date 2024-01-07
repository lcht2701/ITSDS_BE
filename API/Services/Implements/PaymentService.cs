using API.DTOs.Requests.Payments;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Exceptions;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class PaymentService : IPaymentService
{
    private readonly IRepositoryBase<Payment> _paymentRepository;
    private readonly IRepositoryBase<PaymentTerm> _termRepository;
    private readonly IMapper _mapper;

    public PaymentService(IRepositoryBase<Payment> paymentRepository, IRepositoryBase<PaymentTerm> termRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _termRepository = termRepository;
        _mapper = mapper;
    }

    public async Task<List<Payment>> Get()
    {
        var result = await _paymentRepository.GetAsync(navigationProperties: new string[] { "Contract" });
        return result.ToList();
    }

    public async Task<List<Payment>> GetByContract(int contractId)
    {
        var result = await _paymentRepository.WhereAsync(x => x.ContractId == contractId, new string[] { "Contract" });
        return result.ToList();
    }

    public async Task<Payment> GetById(int id)
    {
        var result = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id, new string[] { "Contract" }) ??
               throw new KeyNotFoundException("Payment is not exist");
        return result;
    }

    public async Task<Payment> Create(CreatePaymentRequest model)
    {
        var entity = _mapper.Map<Payment>(model);
        entity.IsFullyPaid = false;
        await _paymentRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<Payment> Update(int id, UpdatePaymentRequest model)
    {
        var target = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id) ??
                     throw new KeyNotFoundException("Payment is not exist");
        var entity = _mapper.Map(model, target);
        await _paymentRepository.UpdateAsync(entity);
        return entity;
    }

    public async Task Remove(int id)
    {
        var target = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id) ??
                     throw new KeyNotFoundException("Payment is not exist");
        var terms = await _termRepository.WhereAsync(x => x.PaymentId == target.Id);
        foreach (var term in terms)
        {
            await _termRepository.DeleteAsync(term);
        }

        await _paymentRepository.DeleteAsync(target);
    }

    public async Task ClosePayment(int id)
    {
        var target = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id) ??
                     throw new KeyNotFoundException("Payment is not exist");
        var terms = await _termRepository.WhereAsync(x => x.PaymentId == id);
        bool allTermsFullyPaid = terms.All(term => term.IsPaid == true);
        if (allTermsFullyPaid)
        {
            target.IsFullyPaid = true;
            target.PaymentFinishTime = DateTime.Now;
        }
        else
        {
            throw new BadRequestException("All Payment Terms need to be FINISHED in order to close the payment");
        }

        await _paymentRepository.UpdateAsync(target);
    }

}