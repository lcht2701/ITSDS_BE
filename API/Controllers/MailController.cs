using API.Services.Interfaces;
using Domain.Entities.Mails;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("/v1/itsds/auth")]
public class MailController : BaseController
{
    private readonly IMailService _mailService;

    public MailController(IMailService _MailService)
    {
        _mailService = _MailService;
    }

    [HttpPost]
    [Route("SendMail")]
    public Task<bool> SendMail(MailData mailData)
    {
        return _mailService.SendMailAsync(mailData);
    }

    [HttpPost]
    [Route("SendHTMLMail")]
    public Task<bool> SendWelcomeMail(HTMLMailData htmlMailData)
    {
        var filePath = Directory.GetCurrentDirectory() + "\\Templates\\Welcome.html";
        return _mailService.SendHTMLMailAsync(htmlMailData, filePath);
    }

    [HttpPost]
    [Route("SendMailWithAttachment")]
    public Task<bool> SendMailWithAttachment([FromForm] MailDataWithAttachment mailDataWithAttachment)
    {
        return _mailService.SendMailWithAttachmentsAsync(mailDataWithAttachment);
    }
}
