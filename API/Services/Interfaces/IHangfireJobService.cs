namespace API.Services.Interfaces;

public interface IHangfireJobService
{
    Task RemoveOldToken(double daysCount);
    Task TicketSummaryNotificationJob();
    Task UpdateStatusOfContract();
    Task NotifyNearEndPayment(double daysCount);
    Task UpdateStatusOfOverduePaymentContract(double overdueDays);
    Task NotifyNearExpiredContract();
}