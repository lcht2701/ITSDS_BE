namespace API.Services.Interfaces
{
    public interface IBackgroundJobService
    {
        Task AssignSupportJob(int ticketId);
        Task CancelAssignSupportJob(string jobId, int ticketId);
        Task CloseTicketJob(int ticketId);
        Task CancelCloseTicketJob(string jobId, int ticketId);
    }
}
