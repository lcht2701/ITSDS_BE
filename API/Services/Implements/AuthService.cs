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


namespace API.Services.Implements;

public class AuthService : IAuthService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly MailSettings _mailSettings;

    public AuthService(IRepositoryBase<User> userRepository, IConfiguration configuration, IMapper mapper, IOptions<MailSettings> mailSettings)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _mapper = mapper;
        _mailSettings = mailSettings.Value;
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

    public async Task ForgotPassword(string email)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Email!.Equals(email));
        if (user == null)
        {
            throw new BadRequestException("Invalid Token");
        }

        user.PasswordResetToken = CreateRandomToken();
        user.ResetTokenExpires = DateTime.Now.AddHours(1);
        await _userRepository.UpdateAsync(user);
        await SendResetLink(user);
    }

    public async Task ResetPassword(int uid, string token, ResetPasswordRequest model)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(uid) && x.PasswordResetToken!.Equals(token));
        if (user == null)
        {
            throw new BadRequestException("User is not found.");
        }
        if (user.ResetTokenExpires < DateTime.Now)
        {
            throw new BadRequestException("Token has been expired. Please initiate a new password reset request if you still need to reset your password.");
        }

        var passwordHasher = new PasswordHasher<User>();
        user.Password = passwordHasher.HashPassword(user, model.NewPassword);
        user.PasswordResetToken = null;
        user.ResetTokenExpires = null;
        await _userRepository.UpdateAsync(user);
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

    private string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    private async Task SendResetLink(User user)
    {
        using (MimeMessage emailMessage = new MimeMessage())
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            string resetPasswordLink = $"{_configuration["AppBaseUrl"]}/reset-password?uid={user.Id}&token={WebUtility.UrlEncode(user.PasswordResetToken)}";
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
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset</title>
</head>
<body>
    <p>Hello {0},</p>
    <p>We received a request to reset your password. Please click on the link below to reset your password:</p>
    <p><a href=""{1}"" target=""_blank"">Reset Password</a></p>
    <p>If you did not initiate this request, you can safely ignore this email.</p>
    <p>Thank you,<br>
    ITSDS</p>
</body>
</html>
";
            emailTemplateText = string.Format(emailTemplateText, fullName, resetPasswordLink);

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
