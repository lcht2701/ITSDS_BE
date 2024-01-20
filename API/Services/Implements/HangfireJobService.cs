using API.Services.Interfaces;
using Domain.Constants.Enums;
using Domain.Entities.Mails;
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
    private readonly IRepositoryBase<Payment> _paymentRepository;
    private readonly IRepositoryBase<DeviceToken> _tokenRepository;
    private readonly IMessagingService _messagingService;
    private readonly MailSettings _mailSettings;

    public HangfireJobService(IRepositoryBase<User> userRepository, IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Contract> contractRepository, IRepositoryBase<Company> companyRepository, IRepositoryBase<CompanyMember> companyMemberRepository, IRepositoryBase<Payment> paymentRepository, IRepositoryBase<DeviceToken> tokenRepository, IMessagingService messagingService, IOptions<MailSettings> mailSettings)
    {
        _userRepository = userRepository;
        _ticketRepository = ticketRepository;
        _contractRepository = contractRepository;
        _companyRepository = companyRepository;
        _companyMemberRepository = companyMemberRepository;
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
                        if (DateTime.Today.Date >= contract.StartDate.Date && DateTime.Today.Date <= contract.EndDate.Date)
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
                        if (DateTime.Today.Date > contract.EndDate.Date)
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
            if (payment.EndDateOfPayment == DateTime.Today.AddDays(-overdueDays))
            {
                contract.Status = ContractStatus.Inactive;
                #region Notify to CompanyAdmins & Managers & Accountants

                var companyAdminIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == contract.CompanyId)).Select(x => x.MemberId);
                var users = (await _userRepository.WhereAsync(x => x.Role == Role.Manager || x.Role == Role.Accountant)).Select(x => x.Id);
                var combinedList = companyAdminIds.Concat(users).ToList();
                foreach (var id in combinedList)
                {
                    await _messagingService.SendNotification("Contract Update", $"Contract {contract.Description} has been inactive. Please update payment in order to continue using services in contract.", id);
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

    public async Task NotifyNearEndPayment(double daysCount)
    {
        var payments = await _paymentRepository.ToListAsync();
        foreach (var payment in payments)
        {
            if (payment.EndDateOfPayment == DateTime.Today.AddDays(-daysCount))
            {
                #region Notify to CompanyAdmins & Managers & Accountants
                var contract = await _contractRepository.FirstOrDefaultAsync(x => x.Id == payment.ContractId);
                var companyAdminIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == contract.CompanyId)).Select(x => x.MemberId);
                var users = (await _userRepository.WhereAsync(x => x.Role == Role.Manager || x.Role == Role.Accountant)).Select(x => x.Id);
                var combinedList = companyAdminIds.Concat(users).ToList();
                foreach (var id in combinedList)
                {
                    await _messagingService.SendNotification("Contract Update", $"Contract {contract.Description} has unpaid payment that last 3 days left. Please update payment in order to use services in contract.", id);
                }
                #endregion
            }
        }
    }
}
