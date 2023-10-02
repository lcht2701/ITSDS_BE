using Domain.Application.AppConfig;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Persistence.Services.Interfaces;
using System.Net.Http;

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
    public Task<bool> SendWelcomeMail(HTMLMailData htmlMailData, string filePath)
    {
        filePath = Directory.GetCurrentDirectory() + "\\Templates\\Welcome.html";
        return _mailService.SendHTMLMailAsync(htmlMailData, filePath);
    }

    [HttpPost]
    [Route("SendMailWithAttachment")]
    public Task<bool> SendMailWithAttachment([FromForm] MailDataWithAttachment mailDataWithAttachment)
    {
        return _mailService.SendMailWithAttachmentsAsync(mailDataWithAttachment);
    }
}
