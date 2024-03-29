﻿using API.DTOs.Requests.Messagings;
using Domain.Models;

namespace API.Services.Interfaces;

public interface IMessagingService
{
    Task<List<Messaging>> GetNotification(int userId);
    Task SendNotification(string title, string message, int userId);
    Task SendChatNotification(SendChatNotificationRequest chatModel);
    Task<Messaging> MarkAsRead(int notificationId);
    Task MarkAsReadAll(int userId);
    Task GetToken(int userId, string token);
    Task RemoveToken(int userId, string? token);
}
