using API.DTOs.Requests.Payments;
using API.DTOs.Responses.Payments;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class PaymentService : IPaymentService
{
    private readonly IRepositoryBase<Payment> _paymentRepository;
    private readonly IRepositoryBase<Contract> _contractRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IMapper _mapper;

    public PaymentService(IRepositoryBase<Payment> paymentRepository, IRepositoryBase<Contract> contractRepository, IAttachmentService attachmentService, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _contractRepository = contractRepository;
        _attachmentService = attachmentService;
        _mapper = mapper;
    }

    public async Task<List<GetPaymentResponse>> Get()
    {
        var payments = await _paymentRepository.GetAsync(navigationProperties: new string[] { "Contract" });
        var result = _mapper.Map(payments, new List<GetPaymentResponse>());
        foreach (var entity in result)
        {
            entity.AttachmentUrls = (await _attachmentService
                .Get(Tables.PAYMENT, entity.Id))
                .Select(x => x.Url)
                .ToList();
        }
        return result;
    }

    public async Task<GetPaymentResponse> GetByContract(int contractId)
    {
        var payment = await _paymentRepository.FirstOrDefaultAsync(x => x.ContractId == contractId, new string[] { "Contract" });
        var result = _mapper.Map(payment, new GetPaymentResponse());
        result.AttachmentUrls = (await _attachmentService
                .Get(Tables.PAYMENT, result.Id))
                .Select(x => x.Url)
                .ToList();
        return result;
    }

    public async Task<GetPaymentResponse> GetById(int id)
    {
        var payment = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id, new string[] { "Contract" }) ??
               throw new KeyNotFoundException("Payment is not exist");
        var result = _mapper.Map(payment, new GetPaymentResponse());
        result.AttachmentUrls = (await _attachmentService
                .Get(Tables.PAYMENT, result.Id))
                .Select(x => x.Url)
                .ToList();
        return result;
    }

    public async Task<Payment> Create(CreatePaymentRequest model)
    {
        var target = await _paymentRepository.FirstOrDefaultAsync(x => x.ContractId == model.ContractId);
        if (target != null)
        {
            throw new BadRequestException("There's already a payment for this contract. Please remove the existing payment before adding new one.");
        }
        var entity = _mapper.Map<Payment>(model);
        entity.EndDateOfPayment = entity.StartDateOfPayment.AddDays(model.DaysAmountForPayment);
        entity.IsFullyPaid = false;
        var result = await _paymentRepository.CreateAsync(entity);
        var contract = await _contractRepository.FirstOrDefaultAsync(x => x.Id == model.ContractId);
        contract.Status = Domain.Constants.Enums.ContractStatus.Active;
        await _contractRepository.UpdateAsync(contract);
        await _attachmentService.Add(Tables.PAYMENT, result.Id, model.AttachmentUrls);
        return result;
    }

    public async Task<Payment> Update(int id, UpdatePaymentRequest model)
    {
        var target = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id) ??
                     throw new KeyNotFoundException("Payment is not exist");
        var entity = _mapper.Map(model, target);
        entity.PaymentFinishTime = entity.IsFullyPaid == true ? DateTime.Now : null;
        var result = await _paymentRepository.UpdateAsync(entity);
        await _attachmentService.Update(Tables.PAYMENT, result.Id, model.AttachmentUrls);
        return result;
    }

    public async Task Remove(int id)
    {
        var target = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id) ??
                     throw new KeyNotFoundException("Payment is not exist");
        await _paymentRepository.DeleteAsync(target);
        var contract = await _contractRepository.FirstOrDefaultAsync(x => x.Id == target.ContractId);
        contract.Status = Domain.Constants.Enums.ContractStatus.Pending;
        await _contractRepository.UpdateAsync(contract);
    }

}