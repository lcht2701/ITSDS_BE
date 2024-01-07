namespace API.Services.Interfaces;

public interface IHangfireJobService
{
    Task RemoveOldToken(double daysCount);
    Task TicketSummaryNotificationJob();
    Task UpdateStatusOfContract();
    Task UpdateStatusOfOverduePaymentContract(double overdueDays);
    Task NotifyNearExpiredContract();
    Task SendPaymentTermNotification(double daysBeforeNotification);

}