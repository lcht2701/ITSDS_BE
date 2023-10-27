using API.DTOs.Requests.Notifications;
using API.Services.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using FirebaseAdmin.Messaging;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class MessagingService : IMessagingService
{
    private readonly IRepositoryBase<Messaging> _messagingRepository;

    public MessagingService(IRepositoryBase<Messaging> messagingRepository)
    {
        _messagingRepository = messagingRepository;
    }

    public async Task<List<Messaging>> Get(int userId)
    {
        var result = await _messagingRepository.GetAsync(x => x.UserId == userId);
        return (List<Messaging>)result;
    }

    public async Task SendNotification(SendNotificationRequest model, int userId)
    {
        string title = "ITSDS";
        var message = new Message()
        {
            Notification = new Notification
            {
                Title = title,
                Body = model.Body,
            },
            Token = model.DeviceToken
        };

        var messaging = FirebaseMessaging.DefaultInstance;
        var result = await messaging.SendAsync(message);

        if (!string.IsNullOrEmpty(result))
        {
            var entity = new Messaging()
            {
                Title = title,
                Body = model.Body,
                UserId = userId,
                IsRead = false
            };
            await _messagingRepository.CreateAsync(entity);
        }
        else
        {
            throw new BadRequestException("Error sending the message.");
        }
    }

    public async Task MarkAsRead(int userId)
    {
        var result = await _messagingRepository.GetAsync(x => x.UserId == userId);
        foreach (var item in result)
        {
            if (item.IsRead == false)
            {
                item.IsRead = true;
                await _messagingRepository.UpdateAsync(item);
            }
        }
    }
}
