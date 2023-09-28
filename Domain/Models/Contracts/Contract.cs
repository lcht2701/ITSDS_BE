using Domain.Constants;
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
        public Contract() { }

        public Guid? AccountantId { get; set; }

        public string? AttachmentURl {  get; set; }

        public DateOnly StartDate { get; set; }

        public int Duration { get; set; }

        public double Value { get; set; }

        public TicketStatus? Status { get; set; }

        public Guid TeamId { get; set; }

        public virtual Team Team { get; set; }

        public Guid CompanyId { get; set; }

        public virtual Company Company { get; set; }

        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; }
        [JsonIgnore]
        public virtual ICollection<ContractDetail> ContractDetails { get; set; }
    }
}
