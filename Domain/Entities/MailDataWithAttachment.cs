using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MailDataWithAttachment : MailData
    {
        public IFormFileCollection EmailAttachments { get; set; }
    }
}
