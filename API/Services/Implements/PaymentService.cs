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
    private readonly IRepositoryBase<Company> _companyRepository;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly MailSettings _mailSettings;
    private readonly IMapper _mapper;

    public PaymentService(IRepositoryBase<Payment> paymentRepository, IRepositoryBase<PaymentTerm> termRepository, IRepositoryBase<Contract> contractRepository, IRepositoryBase<Company> companyRepository, IRepositoryBase<CompanyMember> companyMemberRepository, IRepositoryBase<User> userRepository, IOptions<MailSettings> mailSettings, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _termRepository = termRepository;
        _contractRepository = contractRepository;
        _companyRepository = companyRepository;
        _companyMemberRepository = companyMemberRepository;
        _userRepository = userRepository;
        _mailSettings = mailSettings.Value;
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

    public async Task<List<PaymentTerm>> GetPaymentTerms(int paymentId)
    {
        var list = await _termRepository.WhereAsync(x => x.PaymentId == paymentId, new string[] { "Payment" });
        return list.ToList();
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

    public async Task<List<PaymentTerm>> GeneratePaymentTerms(int paymentId)
    {
        List<PaymentTerm> paymentTerms = new();
        var payment = await _paymentRepository.FirstOrDefaultAsync(x => x.Id == paymentId) ??
                      throw new KeyNotFoundException("Payment is not exist");
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
        var target = await _termRepository.FirstOrDefaultAsync(x => x.Id == id) ??
                     throw new KeyNotFoundException("Payment Term is not exist");
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
        var terms = await _termRepository.WhereAsync(x => x.PaymentId == paymentId) ??
                    throw new KeyNotFoundException("Payment Term is not exist");
        foreach (var term in terms)
        {
            await _termRepository.DeleteAsync(term);
        }
    }

    public async Task SendPaymentNotification(int termId)
    {
        #region GetDetail

        var term = await _termRepository.FirstOrDefaultAsync(x => x.Id.Equals(termId))
                   ?? throw new KeyNotFoundException($"Payment Term with ID {termId} is not exist");

        var payment = await _paymentRepository.FirstOrDefaultAsync(x => x.Id.Equals(term.PaymentId))
                      ?? throw new KeyNotFoundException($"Payment with ID {term.PaymentId} is not exist");

        var contract = await _contractRepository.FirstOrDefaultAsync(x => x.Id.Equals(payment.ContractId))
                       ?? throw new KeyNotFoundException($"Contract with ID {payment.ContractId} is not exist");

        var company = await _companyRepository.FirstOrDefaultAsync(x => x.Id.Equals(contract.CompanyId))
                      ?? throw new KeyNotFoundException($"Company with ID {contract.CompanyId} is not exist");

        var customerAdminIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == company.Id && x.IsCompanyAdmin == true)).Select(x => x.MemberId);

        var customerAdmins = await _userRepository.WhereAsync(x => customerAdminIds.Contains(x.Id))
                            ?? throw new KeyNotFoundException(
                                $"Customer Admin not found for the given termId: {termId}");

        #endregion
        foreach (var customerAdmin in customerAdmins)
        {
            using (MimeMessage emailMessage = new MimeMessage())
            {
                MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                emailMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new MailboxAddress($"{customerAdmin.FirstName} {customerAdmin.LastName}",
                    customerAdmin.Email);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = "Payment Term Notification";
                string emailTemplateText = @"<!DOCTYPE html>
<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    <title>Payment Reminder</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""margin: 0; padding: 0;"">
        <tr>
            <td align=""center"" bgcolor=""#f4f4f4"" style=""padding: 20px 0;"">
                <table role=""presentation"" width=""600"" cellspacing=""0"" cellpadding=""0"" style=""margin: 0; padding: 0; border: 1px solid #0099ff; border-radius: 10px; overflow: hidden;"">
                    <tr>
                        <td align=""center"" bgcolor=""#0099ff"" style=""padding: 40px 0; color: #ffffff; font-size: 28px; font-weight: bold; font-family: Arial, sans-serif;"">
                            Payment Reminder
                        </td>
                    </tr>
                    <tr>
                        <td bgcolor=""#ffffff"" style=""padding: 40px 30px; font-family: Arial, sans-serif; font-size: 16px; color: #333;"">
                            <p>Dear {0},</p>
                            <p>We would like to remind you about an upcoming payment term that needs to be made. Below are the details:</p>
                            <table role=""presentation"" width=""100%"" border=""1"" cellspacing=""0"" cellpadding=""10"" style=""margin: 0; padding: 0;"">
                                <tr>
                                    <th>Payment Term Name</th>
                                    <th>Due Date</th>
                                    <th>Amount Due</th>
                                </tr>
                                <tr>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                </tr>
                            </table>
                            <p>Please ensure that the payment is made by the due date to avoid any disruptions to your service.</p>
                            <p>Thank you for your prompt attention to this matter.</p>
                            <p>Sincerely,</p>
                            <p>ITSDS</p>
                        </td>
                    </tr>
                    <tr>
                        <td bgcolor=""#0099ff"" style=""padding: 20px 0; font-family: Arial, sans-serif; font-size: 14px; color: #ffffff; text-align: center;"">
                            &copy; {4} ITSDS. All rights reserved.
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";
                emailTemplateText = string.Format(emailTemplateText, company.CompanyName, term.Description, term.TermEnd,
                    $"{term.TermAmount:N0} VND", DateTime.Now.Year.ToString());

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = emailTemplateText;
                emailBodyBuilder.TextBody = "Plain Text goes here to avoid marked as spam for some email servers.";

                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                using (SmtpClient mailClient = new SmtpClient())
                {
                    await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port,
                        MailKit.Security.SecureSocketOptions.StartTls);
                    await mailClient.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password);
                    await mailClient.SendAsync(emailMessage);
                    await mailClient.DisconnectAsync(true);
                }
            }
        }
    }
}