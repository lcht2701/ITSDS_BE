using Domain.Constants;
using Domain.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Contracts
{
    public partial class Contract : BaseEntity
    {
        public Contract()
        {
            Payments = new HashSet<Payment>();
            ContractDetails = new HashSet<ContractDetail>();
        }

        public int? AccountantId { get; set; }

        public string? AttachmentURl { get; set; }

        public DateOnly? StartDate { get; set; }

        public int? Duration { get; set; }

        public double? Value { get; set; }

        public TicketStatus? Status { get; set; }

        public int? TeamId { get; set; }

        public int? CompanyId { get; set; }

        [JsonIgnore]
        public virtual Team? Team { get; set; }
        [JsonIgnore]
        public virtual Company? Company { get; set; }
        [JsonIgnore]
        public virtual ICollection<Payment>? Payments { get; set; }
        [JsonIgnore]
        public virtual ICollection<ContractDetail>? ContractDetails { get; set; }
    }
}
