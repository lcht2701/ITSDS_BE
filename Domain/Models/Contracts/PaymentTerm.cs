using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Contracts
{
    public partial class PaymentTerm : BaseEntity
    {
        public PaymentTerm()
        {

        }
        public int? PaymentId { get; set; }

        public virtual Payment? Payment { get; set; }
    }
}
