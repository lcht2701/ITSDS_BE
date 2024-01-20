using API.DTOs.Requests.Messagings;
using API.Services.Interfaces;
using Domain.Models;
using FirebaseAdmin.Messaging;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class MessagingService : IMessagingService
{
    private readonly IRepositoryBase<Messaging> _messagingRepository;
    private readonly IRepositoryBase<DeviceToken> _tokenRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly ILogger<MessagingService> _logger;

    public MessagingService(IRepositoryBase<Messaging> messagingRepository, IRepositoryBase<DeviceToken> tokenRepository, IRepositoryBase<User> userRepository, ILogger<MessagingService> logger)
    {
        _messagingRepository = messagingRepository;
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<List<Messaging>> GetNotification(int userId)
    {
        var result = (await _messagingRepository.WhereAsync(x => x.UserId == userId)).Take(50).OrderByDescending(x => x.CreatedAt).ToList();
        return result;
    }

    public async Task SendNotification(string title, string message, int userId)
    {
        await _messagingRepository.CreateAsync(new Messaging()
        {
            Title = title,
            Body = message,
            UserId = userId,
            IsRead = false
        });

        // Retrieve the device token for the user
        var model = await _tokenRepository.FirstOrDefaultAsync(x => x.UserId == userId);
        if (model == null || string.IsNullOrEmpty(model.Token))
        {
            return;
        }

        try
        {
            var notification = new Message()
            {
                Notification = new Notification
                {
                    Title = title,
                    Body = message,
                },
                Token = model.Token
            };

            var result = await FirebaseMessaging.DefaultInstance.SendAsync(notification);
        }
        catch (Exception ex)
        {
            _logger.ToString();
            return;
        }
    }

    public async Task<Messaging> MarkAsRead(int notificationId)
    {
        var notification = await _messagingRepository.FirstOrDefaultAsync(x => x.Id == notificationId) ?? throw new KeyNotFoundException("Notification is not exist");
        notification.IsRead = true;
        await _messagingRepository.UpdateAsync(notification);
        return notification;
    }

    public async Task SendChatNotification(SendChatNotificationRequest chatModel)
    {
        var user = await _userRepository
            .FirstOrDefaultAsync(x => x.Email == chatModel.Email)
            ?? throw new KeyNotFoundException("User not found");

        // Retrieve the device token for the user
        var model = await _tokenRepository.FirstOrDefaultAsync(x => x.UserId == user.Id);
        if (model == null || string.IsNullOrEmpty(model.Token))
        {
            return;
        }
        try
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            var notification = new Message()
            {
                Notification = new Notification
                {
                    Title = $"Message from [{fullName}]",
                    Body = chatModel.Type == "image" ? "Sent an image" : chatModel.Content,
                },
                Token = model.Token
            };

            var result = await FirebaseMessaging.DefaultInstance.SendAsync(notification);
        }
        catch (Exception ex)
        {
            _logger.ToString();
            return;
        }
    }

    public async Task MarkAsReadAll(int userId)
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

    public async Task GetToken(int userId, string token)
    {
        var existTokens = await _tokenRepository.WhereAsync(x => x.UserId.Equals(token));

        if (existTokens.Any())
        {
            foreach (var item in existTokens)
            {
                await _tokenRepository.DeleteAsync(item);
            }
        }
        else
        {
            var existUserToken = await _tokenRepository.FirstOrDefaultAsync(x => x.UserId.Equals(userId));

            if (existUserToken == null)
            {
                var newToken = new DeviceToken
                {
                    Token = token,
                    UserId = userId,
                };

                await _tokenRepository.CreateAsync(newToken);
            }
            else if (existUserToken.Token != token)
            {
                existUserToken.Token = token;
                await _tokenRepository.UpdateAsync(existUserToken);
            }
        }
    }

    public async Task RemoveToken(int userId, string? token)
    {
        if (token != null)
        {
            var existToken = await _tokenRepository.FirstOrDefaultAsync(x => x.Token!.Equals(token) && x.UserId.Equals(userId));
            if (existToken != null)
            {
                await _tokenRepository.DeleteAsync(existToken);
            }
        }
    }
}
