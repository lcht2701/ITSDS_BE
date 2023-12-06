namespace API.Services.Interfaces;

public interface IHangfireJobService
{
    Task PeriodTicketSummaryNotificationJob();
    Task UpdateStatusOfContract();
}