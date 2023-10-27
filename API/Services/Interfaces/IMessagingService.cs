using API.DTOs.Requests.Notifications;
using Domain.Models;

namespace API.Services.Interfaces;

public interface IMessagingService
{
    Task<List<Messaging>> Get(int userId);
    Task SendNotification(SendNotificationRequest model, int userId);
    Task MarkAsRead(int userId);
}
