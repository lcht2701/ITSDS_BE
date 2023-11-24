using API.DTOs.Requests.Payments;
using API.DTOs.Requests.PaymentTerms;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Application.AppConfig;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Contracts;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class PaymentService : IPaymentService
{
    private readonly IRepositoryBase<Payment> _paymentRepository;
    private readonly IRepositoryBase<PaymentTerm> _termRepository;
    private readonly IRepositoryBase<Contract> _contractRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly MailSettings _mailSettings;
    private readonly IMapper _mapper;

    public PaymentService(IOptions<MailSettings> mailSettings, IRepositoryBase<Payment> paymentRepository, IRepositoryBase<PaymentTerm> termRepository, IRepositoryBase<Contract> contractRepository, IRepositoryBase<User> userRepository, IMapper mapper)
    {
        _mailSettings = mailSettings.Value;
        _paymentRepository = paymentRepository;
        _termRepository = termRepository;
        _contractRepository = contractRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<Payment>> Get()
    {
        return await _paymentRepository.ToListAsync();
    }

    public async Task<Payment> GetByContract(int contractId)
    {
        return await _paymentRepository.FirstOrDefaultAsync(x => x.ContractId == contractId) ?? throw new KeyNotFoundException("Payment is not exist");
    }

    public async Task<Payment> GetById(int id)
    {
        return await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException("Payment is not exist");
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

    public async Task ClosePayment(int id)
    {
        var target = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException("Payment is not exist");
        var terms = await _termRepository.WhereAsync(x => x.PaymentId == id);
        bool allTermsFullyPaid = terms.All(term => term.IsPaid == true);
        if (allTermsFullyPaid)
        {
            target.IsFullyPaid = true;
            target.PaymentFinishTime = DateTime.Now;
        }
        else
        {
            throw new BadRequestException("All Payment Terms need to be updated to Paid in order to close the payment");
        }
        await _paymentRepository.UpdateAsync(target);
    }

    public async Task<List<PaymentTerm>> GeneratePaymentTerms(int paymentId)
    {
        List<PaymentTerm> paymentTerms = new();
        var payment = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == paymentId) ?? throw new KeyNotFoundException("Payment is not exist");
        var contract = await _contractRepository.FirstOrDefaultAsync(x => x.Id == payment.ContractId);
        DateTime paymentDate = payment.FirstDateOfPayment;
        double remainingAmount = (double)contract.Value - payment.InitialPaymentAmount;
        double installmentAmount = remainingAmount / payment.NumberOfTerms;
        for (int i = 0; i < payment.NumberOfTerms; i++)
        {
            paymentTerms.Add(new PaymentTerm
            {
                PaymentId = payment.Id,
                Description = $"Contract {contract.Name} - Payment Number {i + 1}",
                TermAmount = installmentAmount,
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
        var target = await _termRepository.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException("Payment Term is not exist");
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
        var terms = await _termRepository.WhereAsync(x => x.PaymentId == paymentId) ?? throw new KeyNotFoundException("Payment Term is not exist");
        foreach (var term in terms)
        {
            await _termRepository.DeleteAsync(term);
        }
    }

    public async Task<bool> SendPaymentNotification(int termId)
    {
        var term = await _termRepository.FirstOrDefaultAsync(x => x.Id.Equals(termId)) ?? throw new KeyNotFoundException("Payment Term is not exist");
        var company = term.Payment.Contract.Company ?? throw new KeyNotFoundException("Company not found for the given termId");
        var customerAdmin = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(company.CustomerAdminId));

        //var payment = await _paymentRepository.FirstOrDefaultAsync(x => x.Id.Equals(term.PaymentId));
        //var contract = await _contractRepository.FirstOrDefaultAsync(x => x.Id.Equals(payment.ContractId));
        //var company = await _companyRepository.FirstOrDefaultAsync(x => x.Id.Equals(contract.CompanyId));
        try
        {
            using (MimeMessage emailMessage = new MimeMessage())
            {
                MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                emailMessage.From.Add(emailFrom);

                string fullName = $"{customerAdmin.FirstName} {customerAdmin.LastName}";
                MailboxAddress emailTo = new MailboxAddress(fullName, customerAdmin.Email);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = "Hello";

                string emailTemplateText = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Templates\\PaymentNotification.html");

                emailTemplateText = string.Format(emailTemplateText, company.CompanyName, term.Description, term.TermAmount, term.TermEnd, DateTime.Now.Year.ToString());

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = emailTemplateText;
                emailBodyBuilder.TextBody = "Plain Text goes here to avoid marked as spam for some email servers.";

                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                using (SmtpClient mailClient = new SmtpClient())
                {
                    await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    await mailClient.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password);
                    await mailClient.SendAsync(emailMessage);
                    await mailClient.DisconnectAsync(true);
                }
            }

            return true;
        }
        catch (Exception)
        {
            // Exception Details
            return false;
        }
    }
}
