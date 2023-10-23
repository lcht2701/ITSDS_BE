using Domain.Entities.Mails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendMailAsync(MailData mailData);
        Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData, string filePath);
        Task<bool> SendMailWithAttachmentsAsync(MailDataWithAttachment mailDataWithAttachment);
    }
}
