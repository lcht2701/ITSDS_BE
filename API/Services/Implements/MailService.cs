using API.Services.Interfaces;
using Domain.Application.AppConfig;
using Domain.Entities.Mails;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace API.Services.Implements;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;

    public MailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendUserCreatedNotification(string fullname, string username, string email, string password, string roleName)
    {
        using (MimeMessage emailMessage = new MimeMessage())
        {
            MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
            emailMessage.From.Add(emailFrom);
            MailboxAddress emailTo = new MailboxAddress(fullname,
                email);
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
            emailTemplateText = string.Format(emailTemplateText, fullname, username, password, email,
                roleName);

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
