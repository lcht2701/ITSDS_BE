using API.DTOs.Requests.Auths;
using API.DTOs.Responses.Auths;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Application.AppConfig;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Persistence.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MailKit.Net.Smtp;
using Domain.Models.Contracts;

namespace API.Services.Implements;

public class AuthService : IAuthService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly MailSettings _mailSettings;

    public AuthService(IRepositoryBase<User> userRepository, IConfiguration configuration, IMapper mapper, IOptions<MailSettings> mailSettings, IRepositoryBase<CompanyMember> companyMemberRepository)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _mapper = mapper;
        _mailSettings = mailSettings.Value;
        _companyMemberRepository = companyMemberRepository;
    }

    public async Task<LoginResponse> Login(LoginRequest model)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Username!.Equals(model.Username)) ?? throw new KeyNotFoundException("User is not found");
        var passwordHasher = new PasswordHasher<User>();
        var isMatchPassword = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password) == PasswordVerificationResult.Success;
        if (!isMatchPassword)
        {
            throw new UnauthorizedException("Password is incorrect");
        }
        if (user.IsActive == false)
        {
            throw new UnauthorizedException("Your account has been suspended");
        }

        var entity = _mapper.Map(user, new LoginResponse());
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.MemberId.Equals(user.Id));
        entity.IsCustomerAdmin = companyMember != null && companyMember.IsCompanyAdmin;
        entity.AccessToken = GenerateToken(user);
        return entity;
    }

    public async Task<LoginResponse> LoginAdmin(LoginRequest model)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Username!.Equals(model.Username)) ?? throw new KeyNotFoundException("User is not found");
        if (user.IsActive == false)
        {
            throw new UnauthorizedException("Your account has been suspended");
        }

        if (user.Role != Role.Admin)
        {
            throw new UnauthorizedException("You are not allowed to enter");
        }

        var passwordHasher = new PasswordHasher<User>();
        var isMatchPassword = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password) == PasswordVerificationResult.Success;

        if (!isMatchPassword)
        {
            throw new UnauthorizedException("Password is incorrect");
        }

        var entity = _mapper.Map(user, new LoginResponse());
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.MemberId.Equals(user.Id));
        entity.IsCustomerAdmin = companyMember != null && companyMember.IsCompanyAdmin;
        entity.AccessToken = GenerateToken(user);
        return entity;
    }

    public async Task ChangePassword(ChangePasswordRequest model, int userId)
    {
        var user = await _userRepository.FoundOrThrow(u => u.Id.Equals(userId), new KeyNotFoundException("User is not found"));

        var passwordHasher = new PasswordHasher<User>();
        var isMatchPassword = passwordHasher.VerifyHashedPassword(user, user.Password, model.CurrentPassword) == PasswordVerificationResult.Success;
        if (!isMatchPassword)
        {
            throw new BadRequestException("Your current password is incorrect.");
        }
        if (model.NewPassword!.Equals(model.CurrentPassword))
        {
            throw new BadRequestException("New password should not be the same as old password.");
        }
        if (!model.NewPassword.Equals(model.ConfirmNewPassword))
        {
            throw new BadRequestException("Password and Confirm Password does not match.");
        }
        user.Password = passwordHasher.HashPassword(user, model.NewPassword);

        await _userRepository.UpdateAsync(user);
    }

    public async Task ResetPassword(string email)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Email.Equals(email));
        if (user == null)
        {
            throw new BadRequestException("User is not found");
        }

        var passwordHasher = new PasswordHasher<User>();
        string newPassword = CreateRandomPassword(8);
        user.Password = passwordHasher.HashPassword(user, newPassword);
        await _userRepository.UpdateAsync(user);
        await SendNewPassword(user, newPassword);
    }

    private string CreateRandomPassword(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new Random();
        string randomPassword = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
        return randomPassword;

    }

    #region Generate JWT Token
    private string GenerateToken(User user)
    {
        var claims = new[] {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Username!),
                new Claim(ClaimTypes.Role, user.Role.ToString()!)
            };
        //Remember to change back to 2 hours
        //return new JwtSecurityTokenHandler().WriteToken(
        //    GenerateTokenByClaims(claims, DateTime.Now.AddMinutes(120))
        //    );
        return new JwtSecurityTokenHandler().WriteToken(
            GenerateTokenByClaims(claims, DateTime.Now.AddDays(1)));
    }

    private SecurityToken GenerateTokenByClaims(Claim[] claims, DateTime expires)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        return new JwtSecurityToken(_configuration["JWT:Issuer"],
             _configuration["JWT:Audience"],
             claims,
             expires: expires,
             signingCredentials: credentials);
    }
    #endregion

    private async Task SendNewPassword(User user, string newPassword)
    {
        using (MimeMessage emailMessage = new MimeMessage())
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
            emailMessage.From.Add(emailFrom);
            MailboxAddress emailTo = new MailboxAddress(fullName,
                user.Email);
            emailMessage.To.Add(emailTo);

            emailMessage.Subject = "Password Reset Notification";
            string emailTemplateText = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Password Reset Notification!</title>
</head>
<body style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f8f9fa;"">

  <div style=""max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">
    <div style=""background-color: #007bff; color: #fff; padding: 20px; text-align: center; border-top-left-radius: 5px; border-top-right-radius: 5px;"">
      <h1>Password Reset Notification!</h1>
    </div>
    <div style=""padding: 20px;"">
      <p>Dear {0},</p>
      <p>You have request a password reset. Here's your new password:</p>
      <ul style=""list-style-type: none; padding: 0;"">
        <li style=""margin-bottom: 10px;""><strong>Username:</strong> {1}</li>
        <li style=""margin-bottom: 10px;""><strong>Password:</strong> {2}</li>
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
            emailTemplateText = string.Format(emailTemplateText, fullName, user.Username, newPassword);

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
