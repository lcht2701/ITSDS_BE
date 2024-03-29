﻿namespace Domain.Models.Contracts
{
    public partial class Payment : BaseEntity
    {
        public Payment()
        {
        }

        public int ContractId { get; set; }

        public string? Description { get; set; }

        public DateTime StartDateOfPayment { get; set; }

        public DateTime EndDateOfPayment { get; set; }

        public bool IsFullyPaid { get; set; }

        public DateTime? PaymentFinishTime { get; set; }

        public string? Note { get; set; }

        public virtual Contract? Contract { get; set; }
    }
}
