using API.DTOs.Requests.CompanyMembers;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Application.AppConfig;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class CompanyMemberService : ICompanyMemberService
{
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly MailSettings _mailSettings;
    private readonly IMapper _mapper;
    private readonly IFirebaseService _firebaseService;

    public CompanyMemberService(IRepositoryBase<CompanyMember> companyMemberRepository, IRepositoryBase<User> userRepository, IMapper mapper, IOptions<MailSettings> mailSettings, IFirebaseService firebaseService)
    {
        _companyMemberRepository = companyMemberRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _mailSettings = mailSettings.Value;
        _firebaseService = firebaseService;
    }

    public async Task<List<CompanyMember>> Get(int userId)
    {
        List<CompanyMember> result = new();
        var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
        var memberInfo = await _companyMemberRepository.FirstOrDefaultAsync(x => x.MemberId == userId);
        switch (currentUser.Role)
        {

            case Role.Customer:
                {
                    result = (await _companyMemberRepository.WhereAsync(x => x.CompanyId.Equals(memberInfo.CompanyId), new string[] { "Member", "Company" })).ToList();
                    break;
                }
            default:
                {
                    result = (await _companyMemberRepository.GetAsync(navigationProperties: new string[] { "Member", "Company" })).ToList();
                    break;
                }
        }
        return result;
    }

    public async Task<List<CompanyMember>> GetCompanyAdmins(int companyId)
    {
        var result = (await _companyMemberRepository.WhereAsync(x => x.CompanyId.Equals(companyId) && x.IsCompanyAdmin, new string[] { "Member", "Company" })).ToList()
            ?? throw new KeyNotFoundException("Company is not exist");
        return result;
    }

    public async Task<List<User>> GetMemberNotInCompany(int companyId)
    {
        var companyMemberIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == companyId)).Select(x => x.MemberId).ToList()
            ?? throw new KeyNotFoundException("Company is not exist");
        var users = (await _userRepository.WhereAsync(x =>
                    !companyMemberIds.Contains(x.Id) &&
                    x.Role == Role.Customer, new string[] { "Member", "Company" }))
                    .ToList();
        return users;
    }

    public async Task<CompanyMember> GetById(int id)
    {
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Member", "Company" }) ?? throw new KeyNotFoundException("Member is not exist");
        return companyMember;
    }

    public async Task<CompanyMember> Add(AddCompanyMemberRequest model, int currentUserId)
    {
        CompanyMember currentUserMember = await IsCompanyAdmin(currentUserId);
        var userAccount = _mapper.Map(model.User, new User());

        //Default when create new account
        userAccount.Role = Role.Customer;
        var passwordHasher = new PasswordHasher<User>();
        userAccount.Password = passwordHasher.HashPassword(userAccount, userAccount.Password);
        userAccount.IsActive = true;

        var userResult = await _userRepository.CreateAsync(userAccount);
        var member = new CompanyMember()
        {
            MemberId = userResult.Id,
            CompanyId = currentUserMember.CompanyId,
            IsCompanyAdmin = model.IsCompanyAdmin,
            MemberPosition = model.MemberPosition != null ? model.MemberPosition : "",
            DepartmentId = model.DepartmentId
        };
        await _companyMemberRepository.CreateAsync(member);
        BackgroundJob.Enqueue(() => SendUserCreatedNotification(model.User!));
        return member;
    }

    public async Task<CompanyMember> Update(int id, UpdateCompanyMemberRequest model, int currentUserId)
    {
        CompanyMember currentUserMember = await IsCompanyAdmin(currentUserId);
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Member is not exist");
        var entity = _mapper.Map(model, companyMember);
        await _companyMemberRepository.UpdateAsync(entity);
        return entity;
    }

    public async Task Remove(int id, int currentUserId)
    {
        CompanyMember currentUserMember = await IsCompanyAdmin(currentUserId);
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Member is not exist");
        var userAccount = await _userRepository.FirstOrDefaultAsync(x => x.Id == currentUserId);
        await _userRepository.DeleteAsync(userAccount);
        await _firebaseService.RemoveFirebaseAccount(userAccount.Id);
        await _companyMemberRepository.DeleteAsync(companyMember);
    }

    public async Task SendUserCreatedNotification(AddAccountInformationRequest dto)
    {
        using (MimeMessage emailMessage = new MimeMessage())
        {
            string fullname = $"{dto.FirstName} {dto.LastName}";
            string role = DataResponse.GetEnumDescription(Role.Customer);
            MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
            emailMessage.From.Add(emailFrom);
            MailboxAddress emailTo = new MailboxAddress(fullname,
                dto.Email);
            emailMessage.To.Add(emailTo);

            emailMessage.Subject = "Welcome to ITSDS - Your New Account Details";
            string emailTemplateText = @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Welcome to ITSDS System!</title>
</head>
<body style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f8f9fa;"">

  <div style=""max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">
    <div style=""background-color: #007bff; color: #fff; padding: 20px; text-align: center; border-top-left-radius: 5px; border-top-right-radius: 5px;"">
      <h1>Welcome to ITSDS System!</h1>
    </div>
    <div style=""padding: 20px;"">
      <p>Dear {0},</p>
      <p>We are delighted to welcome you to ITSDS! Your new account has been successfully created, and we're excited to have you on board. Below are your account details:</p>
      <ul style=""list-style-type: none; padding: 0;"">
        <li style=""margin-bottom: 10px;""><strong>Username:</strong> {1}</li>
        <li style=""margin-bottom: 10px;""><strong>Password:</strong> {2}</li>
        <li style=""margin-bottom: 10px;""><strong>Email:</strong> {3}</li>
        <li style=""margin-bottom: 10px;""><strong>Role:</strong> {4}</li>
      </ul>
      <p>Please keep this information secure, and do not share your password with anyone. If you have any questions or concerns regarding your account, feel free to contact our support team at <a href=""mailto:itsdskns@gmail.com"">itsdskns@gmail.com</a>.</p>
      <p>We recommend logging in at <a href=""https://dichvuit.hisoft.vn/"">https://dichvuit.hisoft.vn/</a> to update your password for added security.</p>
    </div>
    <div style=""background-color: #f1f1f1; padding: 10px; text-align: center; border-bottom-left-radius: 5px; border-bottom-right-radius: 5px;"">
      <p>Best regards, ITSDS</p>
    </div>
  </div>

</body>
</html>
";
            emailTemplateText = string.Format(emailTemplateText, fullname, dto.Username, dto.Password, dto.Email,
                role);

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

    private async Task<CompanyMember> IsCompanyAdmin(int currentUserId)
    {
        var currentUserMember = await _companyMemberRepository
                                            .FirstOrDefaultAsync(x =>
                                                            x.MemberId.Equals(currentUserId) &&
                                                            x.IsCompanyAdmin == true);
        if (currentUserMember == null)
        {
            throw new UnauthorizedAccessException("User is not authorize for this action");
        }

        return currentUserMember;
    }
}
