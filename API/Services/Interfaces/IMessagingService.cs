using Domain.Models;

namespace API.Services.Interfaces;

public interface IMessagingService
{
    Task<List<Messaging>> GetNotification(int userId);
    Task SendNotification(string message, int userId);
    Task MarkAsRead(int userId);
    Task GetToken(int userId, string token);
    Task RemoveToken(int userId, string token);
    Task RemoveOldToken();
}
