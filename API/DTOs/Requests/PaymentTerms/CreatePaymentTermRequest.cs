namespace API.DTOs.Requests.PaymentTerms
{
    public class CreatePaymentTermRequest
    {
        public int PaymentId { get; set; }

        public int NumberOfPayments { get; set; }

        public double InitialPaymentAmount { get; set; }

        public DateTime FirstDateOfPayment { get; set; }

        public int Duration { get; set; }
    }
}
