namespace API.Services.Interfaces;

public interface IHangfireJobService
{
    Task TicketSummaryNotificationJob();
    Task UpdateStatusOfContract();
    Task NotifyNearExpiredContract();
}