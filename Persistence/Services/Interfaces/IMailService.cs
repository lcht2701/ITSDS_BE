using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendMailAsync(MailData mailData);
        Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData);
        Task<bool> SendMailWithAttachmentsAsync(MailDataWithAttachment mailDataWithAttachment);
    }
}
