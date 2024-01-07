using API.DTOs.Requests.PaymentTerms;
using API.DTOs.Responses.PaymentTerms;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class PaymentTermService : IPaymentTermService
{
    private readonly IRepositoryBase<Payment> _paymentRepository;
    private readonly IRepositoryBase<PaymentTerm> _termRepository;
    private readonly IRepositoryBase<Contract> _contractRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IMessagingService _messagingService;
    private readonly IMapper _mapper;

    public PaymentTermService(IRepositoryBase<Payment> paymentRepository, IRepositoryBase<PaymentTerm> termRepository, IRepositoryBase<Contract> contractRepository, IAttachmentService attachmentService, IMessagingService messagingService, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _termRepository = termRepository;
        _contractRepository = contractRepository;
        _attachmentService = attachmentService;
        _messagingService = messagingService;
        _mapper = mapper;
    }

    public async Task<List<GetPaymentTermResponse>> GetPaymentTerms(int paymentId)
    {
        var list = await _termRepository.WhereAsync(x => x.PaymentId == paymentId, new string[] { "Payment" });
        var response = _mapper.Map(list, new List<GetPaymentTermResponse>());
        foreach (var item in response)
        {
            item.AttachmentUrls = (await _attachmentService.Get(Tables.PAYMENTTERMS, item.Id)).Select(x => x.Url).ToList();
        }
        return response;
    }

    public async Task<GetPaymentTermResponse> GetPaymentTermById(int termId)
    {
        var result = await _termRepository.FirstOrDefaultAsync(x => x.Id == termId, new string[] { "Payment" }) 
            ?? throw new KeyNotFoundException("Payment term is not found");
        var response = _mapper.Map(result, new GetPaymentTermResponse());
        response.AttachmentUrls = (await _attachmentService.Get(Tables.PAYMENTTERMS, response.Id)).Select(x => x.Url).ToList();
        return response;
    }

    public async Task<List<PaymentTerm>> GeneratePaymentTerms(int paymentId)
    {
        List<PaymentTerm> paymentTerms = new();
        var payment = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == paymentId) ??
                      throw new KeyNotFoundException("Payment is not exist");
        var contract = await _contractRepository.FirstOrDefaultAsync(x => x.Id == payment.ContractId);
        DateTime paymentDate = payment.FirstDateOfPayment;
        double remainingAmount = contract.Value - payment.InitialPaymentAmount;
        double amountPerTeem = remainingAmount / payment.NumberOfTerms;
        for (int i = 0; i < payment.NumberOfTerms; i++)
        {
            paymentTerms.Add(new PaymentTerm
            {
                PaymentId = payment.Id,
                TermAmount = amountPerTeem,
                TermStart = paymentDate,
                TermEnd = paymentDate.AddDays(15),
                IsPaid = false,
            });
            int monthsAdded = payment.Duration / payment.NumberOfTerms;
            paymentDate = paymentDate.AddMonths(monthsAdded);
        }

        await _termRepository.CreateAsync(paymentTerms);
        return paymentTerms;
    }

    public async Task<PaymentTerm> UpdatePaymentTerm(int id, UpdatePaymentTermRequest model)
    {
        var target = await _termRepository.FirstOrDefaultAsync(x => x.Id == id) ??
                     throw new KeyNotFoundException("Payment Term is not exist");
        var entity = _mapper.Map(model, target);
        if (entity.IsPaid == true)
        {
            entity.TermFinishTime = DateTime.Now;
        }
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Update(Tables.PAYMENTTERMS, id, model.AttachmentUrls);
        }
        await _termRepository.UpdateAsync(entity);

        #region Update Payment & Contract
        var unpaidTerms = await _termRepository
            .WhereAsync(x => x.PaymentId == entity.PaymentId && !x.IsPaid);

        var payment = await _paymentRepository
            .FirstOrDefaultAsync(x => x.Id == entity.PaymentId);
        var contract = await _contractRepository
            .FirstOrDefaultAsync(x => x.Id == payment.ContractId);

        if (!unpaidTerms.Any())
        {
            // All terms are paid
            payment.IsFullyPaid = true;
            payment.PaymentFinishTime = DateTime.Now;
            await _paymentRepository.UpdateAsync(payment);

            if (contract.Status == ContractStatus.Inactive)
            {
                // Update contract status to Active if it's inactive
                contract.Status = ContractStatus.Active;
                await _contractRepository.UpdateAsync(contract);
            }
        }
        else
        {
            // Some terms are still unpaid
            payment.IsFullyPaid = false;
            payment.PaymentFinishTime = null;
            await _paymentRepository.UpdateAsync(payment);
        }


        #endregion

        return entity;
    }

    public async Task RemovePaymentTerm(int paymentId)
    {
        var terms = await _termRepository.WhereAsync(x => x.PaymentId == paymentId) ??
                    throw new KeyNotFoundException("Payment Term is not exist");
        foreach (var term in terms)
        {
            await _attachmentService.Delete(Tables.PAYMENTTERMS, term.Id);
            await _termRepository.DeleteAsync(term);
        }
    }
}
