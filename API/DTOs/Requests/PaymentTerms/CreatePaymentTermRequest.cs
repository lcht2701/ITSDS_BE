using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.PaymentTerms
{
    public class CreatePaymentTermRequest
    {
        [Required]
        public int PaymentId { get; set; }

        [Required]
        public int NumberOfPayments { get; set; }

        [Required]
        public double InitialPaymentAmount { get; set; }

        [Required]
        public DateTime FirstDateOfPayment { get; set; }

        [Required]
        public int Duration { get; set; }
    }
}
