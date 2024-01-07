using API.Services.Interfaces;
using Domain.Application.AppConfig;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class HangfireJobService : IHangfireJobService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Contract> _contractRepository;
    private readonly IRepositoryBase<Company> _companyRepository;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IRepositoryBase<PaymentTerm> _termRepository;
    private readonly IRepositoryBase<Payment> _paymentRepository;
    private readonly IRepositoryBase<DeviceToken> _tokenRepository;
    private readonly IMessagingService _messagingService;
    private readonly MailSettings _mailSettings;

    public HangfireJobService(IRepositoryBase<User> userRepository, IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Contract> contractRepository, IRepositoryBase<Company> companyRepository, IRepositoryBase<CompanyMember> companyMemberRepository, IRepositoryBase<PaymentTerm> termRepository, IRepositoryBase<Payment> paymentRepository, IRepositoryBase<DeviceToken> tokenRepository, IMessagingService messagingService, IOptions<MailSettings> mailSettings)
    {
        _userRepository = userRepository;
        _ticketRepository = ticketRepository;
        _contractRepository = contractRepository;
        _companyRepository = companyRepository;
        _companyMemberRepository = companyMemberRepository;
        _termRepository = termRepository;
        _paymentRepository = paymentRepository;
        _tokenRepository = tokenRepository;
        _messagingService = messagingService;
        _mailSettings = mailSettings.Value;
    }

    public async Task RemoveOldToken(double daysCount)
    {
        var listTokens = await _tokenRepository.ToListAsync();

        foreach (var token in listTokens)
        {
            TimeSpan timeDifference;
            if (token.ModifiedAt != null)
            {
                timeDifference = (TimeSpan)(DateTime.UtcNow - token.ModifiedAt);
            }
            else
            {
                timeDifference = (TimeSpan)(DateTime.UtcNow - token.CreatedAt)!;
            }

            // Remove tokens older than 7 days
            if (timeDifference.TotalDays >= daysCount)
            {
                await _tokenRepository.DeleteAsync(token);
            }
        }
    }

    public async Task TicketSummaryNotificationJob()
    {
        if (DateTime.Today.DayOfWeek != DayOfWeek.Monday)
        {
            return;
        }

        var managerList = await _userRepository.WhereAsync(x => x.Role.Equals(Role.Manager));

        foreach (var manager in managerList)
        {
            var availableTicketsCount = (await _ticketRepository
                .WhereAsync(x => (x.TicketStatus != TicketStatus.Closed && x.TicketStatus != TicketStatus.Cancelled) &&
                                 x.ScheduledStartTime >= DateTime.Today &&
                                 x.ScheduledStartTime <= DateTime.Today.AddDays(7)))
                .Count;

            if (availableTicketsCount > 0)
            {
                await _messagingService.SendNotification(
                    "Ticket Summary",
                    $"There are still {availableTicketsCount} available tickets that need to be handled this week.",
                    manager.Id
                );
            }
        }
    }

    public async Task UpdateStatusOfContract()
    {
        var contracts = await _contractRepository.ToListAsync();
        foreach (var contract in contracts)
        {
            switch (contract.Status)
            {
                case ContractStatus.Pending:
                    {
                        if (contract.StartDate >= DateTime.Today)
                        {
                            contract.Status = ContractStatus.Active;
                            await _contractRepository.UpdateAsync(contract);
                            #region Notify to CompanyAdmins & Managers & Accountants

                            var companyAdminIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == contract.CompanyId)).Select(x => x.MemberId);
                            var users = (await _userRepository.WhereAsync(x => x.Role == Role.Manager || x.Role == Role.Accountant)).Select(x => x.Id);
                            var combinedList = companyAdminIds.Concat(users).ToList();
                            foreach (var id in combinedList)
                            {
                                await _messagingService.SendNotification("Contract Update", $"Contract {contract.Description} has been updated to Active stage.", id);
                            }
                            #endregion
                        }
                        break;
                    }
                case ContractStatus.Active:
                case ContractStatus.Inactive:
                    {
                        if (contract.EndDate >= DateTime.Now)
                        {
                            contract.Status = ContractStatus.Expired;
                            await _contractRepository.UpdateAsync(contract);
                            #region Notify to CompanyAdmins & Managers & Accountants

                            var companyAdminIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == contract.CompanyId)).Select(x => x.MemberId);
                            var users = (await _userRepository.WhereAsync(x => x.Role == Role.Manager || x.Role == Role.Accountant)).Select(x => x.Id);
                            var combinedList = companyAdminIds.Concat(users).ToList();
                            foreach (var id in combinedList)
                            {
                                await _messagingService.SendNotification("Contract Update", $"Contract {contract.Description} has been expired.", id);
                            }
                            #endregion
                        }
                        break;
                    }
            }
        }
    }

    public async Task UpdateStatusOfOverduePaymentContract(double overdueDays)
    {
        var contracts = await _contractRepository.WhereAsync(x => x.Status == ContractStatus.Active);
        foreach (var contract in contracts)
        {
            var payment = await _paymentRepository.FirstOrDefaultAsync(x => x.ContractId.Equals(contract.Id));
            var overduePaymentTerms = await _termRepository
                .WhereAsync(x =>
                    x.IsPaid == false &&
                    x.TermEnd == DateTime.Today.AddDays(-overdueDays));
            if (overduePaymentTerms.Any())
            {
                contract.Status = ContractStatus.Inactive;
                #region Notify to CompanyAdmins & Managers & Accountants

                var companyAdminIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == contract.CompanyId)).Select(x => x.MemberId);
                var users = (await _userRepository.WhereAsync(x => x.Role == Role.Manager || x.Role == Role.Accountant)).Select(x => x.Id);
                var combinedList = companyAdminIds.Concat(users).ToList();
                foreach (var id in combinedList)
                {
                    await _messagingService.SendNotification("Contract Update", $"Contract {contract.Description} has been updated to Active stage.", id);
                }
                #endregion
            }
        }
    }

    public async Task NotifyNearExpiredContract()
    {
        var nearExpiredContracts = await _contractRepository.WhereAsync(x => x.EndDate.Date == DateTime.Today.AddMonths(-1));
        foreach (var contract in nearExpiredContracts)
        {
            var company = await _companyRepository.FirstOrDefaultAsync(x => x.Id == contract.CompanyId);
            var companyAdminIds = (await _companyMemberRepository.WhereAsync(x => x.Id == contract.CompanyId && x.IsCompanyAdmin == true)).Select(x => x.MemberId);
            var companyAdmins = await _userRepository.WhereAsync(x => companyAdminIds.Contains(x.Id));
            if (companyAdmins == null)
                return;
            foreach (var companyAdmin in companyAdmins)
            {
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    var companyAdminName = $"{companyAdmin.FirstName} {companyAdmin.LastName}";
                    MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                    emailMessage.From.Add(emailFrom);
                    MailboxAddress emailTo = new MailboxAddress(companyAdminName,
                        companyAdmin.Email);
                    emailMessage.To.Add(emailTo);

                    emailMessage.Subject = "Contract Expiry Notification";
                    string emailTemplateText = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Contract Expiry Notification</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
        }

        .notification {
            border: 2px solid #f44336;
            border-radius: 5px;
            padding: 20px;
            background-color: #f8d7da;
            color: #721c24;
        }

        .contract-details {
            margin-top: 10px;
        }

        .contract-details p {
            margin: 5px 0;
        }
    </style>
</head>
<body>

    <div class=""notification"">
        <h2>Contract Expiry Notification</h2>
        <p>Your contract is near expiration. Please review and take necessary actions.</p>
        <div class=""contract-details"">
            <p><strong>Name:</strong> {0}</p>
            <p><strong>Description:</strong> {1}</p>
            <p><strong>Start Date:</strong> {2}</p>
            <p><strong>End Date:</strong> {3}</p>
            <p><strong>Company Name:</strong> {4}</p>
        </div>
    </div>
</body>
</html>
";

                    emailTemplateText = string.Format(emailTemplateText, contract.Name, contract.Description, contract.StartDate.ToString(), contract.EndDate.ToString(), company.CompanyName);

                    BodyBuilder emailBodyBuilder = new BodyBuilder();
                    emailBodyBuilder.HtmlBody = emailTemplateText;
                    emailBodyBuilder.TextBody = "Plain Text goes here to avoid marked as spam for some email servers.";

                    emailMessage.Body = emailBodyBuilder.ToMessageBody();

                    using (SmtpClient mailClient = new SmtpClient())
                    {
                        await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port,
                            SecureSocketOptions.StartTls);
                        await mailClient.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password);
                        await mailClient.SendAsync(emailMessage);
                        await mailClient.DisconnectAsync(true);
                    }
                }

            }
        }
    }

    public async Task SendPaymentTermNotification(double daysBeforeNotification)
    {


        var terms = await _termRepository.WhereAsync(x => x.TermStart == DateTime.Today.AddDays(daysBeforeNotification));

        foreach (var term in terms)
        {
            #region GetDetail
            var payment = await _paymentRepository.FirstOrDefaultAsync(x => x.Id.Equals(term.PaymentId))
                                  ?? throw new KeyNotFoundException($"Payment with ID {term.PaymentId} is not exist");

            var contract = await _contractRepository.FirstOrDefaultAsync(x => x.Id.Equals(payment.ContractId))
                           ?? throw new KeyNotFoundException($"Contract with ID {payment.ContractId} is not exist");

            var company = await _companyRepository.FirstOrDefaultAsync(x => x.Id.Equals(contract.CompanyId))
                          ?? throw new KeyNotFoundException($"Company with ID {contract.CompanyId} is not exist");

            var companyAdminIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == company.Id && x.IsCompanyAdmin == true)).Select(x => x.MemberId);

            var companyAdmins = await _userRepository.WhereAsync(x => companyAdminIds.Contains(x.Id))
                                ?? throw new KeyNotFoundException(
                                    $"Customer Admin not found for the given termId: {term.Id}");

            #endregion
            foreach (var companyAdmin in companyAdmins)
            {
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                    emailMessage.From.Add(emailFrom);
                    MailboxAddress emailTo = new MailboxAddress($"{companyAdmin.FirstName} {companyAdmin.LastName}",
                        companyAdmin.Email);
                    emailMessage.To.Add(emailTo);

                    emailMessage.Subject = "Payment Term Notification";
                    string emailTemplateText = @"
<!DOCTYPE html>
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
                                    <th>Contract</th>
                                    <th>Amount Due</th>
                                    <th>Due Date</th>
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
                    emailTemplateText = string.Format(emailTemplateText, company.CompanyName, contract.Name, term.TermEnd,
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
}
