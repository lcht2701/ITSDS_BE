using API.DTOs.Requests.Payments;
using API.DTOs.Requests.PaymentTerms;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Contracts;
using Google.Protobuf.WellKnownTypes;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class PaymentService : IPaymentService
{
    private readonly IRepositoryBase<Payment> _paymentRepository;
    private readonly IRepositoryBase<PaymentTerm> _termRepository;
    private readonly IRepositoryBase<Contract> _contractRepository;
    private readonly IMapper _mapper;

    public PaymentService(IRepositoryBase<Payment> paymentRepository, IRepositoryBase<PaymentTerm> termRepository, IRepositoryBase<Contract> contractRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _termRepository = termRepository;
        _contractRepository = contractRepository;
        _mapper = mapper;
    }

    public async Task<List<Payment>> Get()
    {
        return await _paymentRepository.ToListAsync();
    }

    public async Task<Payment> GetByContract(int contractId)
    {
        return await _paymentRepository.FirstOrDefaultAsync(x => x.ContractId == contractId);
    }

    public async Task<Payment> GetById(int id)
    {
        return await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<PaymentTerm>> GetPaymentTerms(int paymentId)
    {
        var list = (await _termRepository.WhereAsync(x => x.PaymentId == paymentId)).ToList();
        return list;
    }

    public async Task<Payment> Create(CreatePaymentRequest model)
    {
        var entity = _mapper.Map<Payment>(model);
        await _paymentRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<Payment> Update(int id, UpdatePaymentRequest model)
    {
        var target = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException("Payment is not exist");
        var entity = _mapper.Map(model, target);
        if (entity.IsFullyPaid == true && entity.PaymentFinishTime == null)
        {
            entity.PaymentFinishTime = DateTime.UtcNow;
        }
        await _paymentRepository.UpdateAsync(entity);
        return entity;

    }

    public async Task Remove(int id)
    {
        var target = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException("Payment is not exist");
        var terms = await _termRepository.WhereAsync(x => x.PaymentId == target.Id);
        foreach (var term in terms)
        {
            await _termRepository.DeleteAsync(term);
        }
        await _paymentRepository.DeleteAsync(target);
    }

    public async Task<List<PaymentTerm>> CreatePaymentTerm(CreatePaymentTermRequest model)
    {
        List<PaymentTerm> paymentTerms = new();
        var payment = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == model.PaymentId) ?? throw new KeyNotFoundException("Payment is not exist");
        var contract = await _contractRepository.FirstOrDefaultAsync(x => x.Id == payment.ContractId);
        if (model.FirstDateOfPayment < payment.PaymentStart)
        {
            throw new BadHttpRequestException("First Date Of Payment Term must not be earlier than Payment Date");
        }
        DateTime paymentDate = model.FirstDateOfPayment;
        double remainingAmount = (double)contract.Value - model.InitialPaymentAmount;
        double installmentAmount = remainingAmount / model.NumberOfPayments;
        for (int i = 0; i < model.NumberOfPayments; i++)
        {
            paymentTerms.Add(new PaymentTerm
            {
                PaymentId = payment.Id,
                Description = $"Contract {contract.Name} - Payment Number {i+1}",
                TermAmount = installmentAmount,
                TermStart = paymentDate,
                TermEnd = paymentDate.AddDays(15),
                IsPaid = false,
            });
            paymentDate = paymentDate.AddMonths(model.Duration /  model.NumberOfPayments);
        }
        await _termRepository.CreateAsync(paymentTerms);
        return paymentTerms;
    }

    public async Task<PaymentTerm> UpdatePaymentTerm(int paymentId, UpdatePaymentTermRequest model)
    {
        var target = await _termRepository.FirstOrDefaultAsync(x => x.Id == paymentId) ?? throw new KeyNotFoundException("Payment Term is not exist");
        var entity = _mapper.Map(model, target);
        if (entity.IsPaid == true)
        {
            entity.TermFinishTime = DateTime.UtcNow;
        }
        await _termRepository.UpdateAsync(entity);
        return entity;
    }

    public async Task RemovePaymentTerm(int paymentId)
    {
        var target = await _termRepository.FirstOrDefaultAsync(x => x.Id == paymentId) ?? throw new KeyNotFoundException("Payment Term is not exist");
        await _termRepository.DeleteAsync(target);
    }


}
