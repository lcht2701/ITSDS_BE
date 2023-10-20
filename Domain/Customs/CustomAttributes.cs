using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Customs
{
    public class CustomAttributes
    {
        [AttributeUsage(AttributeTargets.Property)]
        public class ExcludeFromAuditLogAttribute : Attribute
        {
        }
    }
}
