namespace Domain.Entities.Mails
{
    public class PaymentNotificationData
    {
        public string? CustomerAdminFullName { get; set; }
        public string? PaymentTermName { get; set; }
        public string? DueDate { get; set; }
        public string? Amount { get; set; }
        public string? MyCompanyName { get; set; }
        public string? Year { get; set; }
    }
}
