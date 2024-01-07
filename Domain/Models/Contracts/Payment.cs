﻿using System.Text.Json.Serialization;

namespace Domain.Models.Contracts
{
    public partial class Payment : BaseEntity
    {
        public Payment()
        {
            PaymentTerms = new HashSet<PaymentTerm>();
        }

        public int ContractId { get; set; }

        public string? Description { get; set; }

        public DateTime FirstDateOfPayment { get; set; }

        public int NumberOfTerms { get; set; }

        public int Duration { get; set; }

        public double InitialPaymentAmount { get; set; }

        public bool IsFullyPaid { get; set; }

        public DateTime? PaymentFinishTime { get; set; }

        public string? Note { get; set; }

        public virtual Contract? Contract { get; set; }

        [JsonIgnore]
        public virtual ICollection<PaymentTerm>? PaymentTerms { get; set; }
    }
}
