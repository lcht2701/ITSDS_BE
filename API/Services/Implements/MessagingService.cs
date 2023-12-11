using API.Services.Interfaces;
using Domain.Models;
using FirebaseAdmin.Messaging;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class MessagingService : IMessagingService
{
    private readonly IRepositoryBase<Messaging> _messagingRepository;
    private readonly IRepositoryBase<DeviceToken> _tokenRepository;
    private readonly ILogger<MessagingService> _logger;

    public MessagingService(IRepositoryBase<Messaging> messagingRepository, IRepositoryBase<DeviceToken> tokenRepository, ILogger<MessagingService> logger)
    {
        _messagingRepository = messagingRepository;
        _tokenRepository = tokenRepository;
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

    public async Task GetToken(int userId, string? token)
    {
        if (token != null)
        {
            var existToken = await _tokenRepository.FirstOrDefaultAsync(x => x.Token!.Equals(token) && x.UserId.Equals(userId));
            if (existToken == null)
            {
                var newToken = new DeviceToken()
                {
                    Token = token,
                    UserId = userId,
                };
                await _tokenRepository.CreateAsync(newToken);
            }
            else
            {
                existToken.UserId = userId;
                existToken.Token = token;
                await _tokenRepository.UpdateAsync(existToken);
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

    public async Task RemoveOldToken()
    {
        var listTokens = await _tokenRepository.ToListAsync();

        foreach (var token in listTokens)
        {
            TimeSpan timeDifference;
            if (token.ModifiedAt != null)
            {
                timeDifference = (TimeSpan)(DateTime.UtcNow - token.ModifiedAt);
            }
            else
            {
                timeDifference = (TimeSpan)(DateTime.UtcNow - token.CreatedAt);
            }

            // Remove tokens older than 7 days
            if (timeDifference.TotalDays > 7)
            {
                await _tokenRepository.DeleteAsync(token);
            }
        }
    }
}
