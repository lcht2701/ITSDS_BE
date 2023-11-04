using API.Services.Interfaces;
using Domain.Models;
using FirebaseAdmin.Messaging;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class MessagingService : IMessagingService
{
    private readonly IRepositoryBase<Messaging> _messagingRepository;
    private readonly IRepositoryBase<DeviceToken> _tokenRepository;

    public MessagingService(IRepositoryBase<Messaging> messagingRepository, IRepositoryBase<DeviceToken> tokenRepository)
    {
        _messagingRepository = messagingRepository;
        _tokenRepository = tokenRepository;
    }

    public async Task<List<Messaging>> GetNotification(int userId)
    {
        var result = await _messagingRepository.GetAsync(x => x.UserId == userId);
        return (List<Messaging>)result;
    }

    public async Task SendNotification(string title, string message, int userId)
    {
        var tokenModel = await _tokenRepository.FirstOrDefaultAsync(x => x.UserId == userId);
        string? token = tokenModel?.Token;

        var entity = new Messaging()
        {
            Title = title,
            Body = message,
            UserId = userId,
            IsRead = false
        };

        if (!string.IsNullOrEmpty(token))
        {
            var notification = new Message()
            {
                Notification = new Notification
                {
                    Title = title,
                    Body = message,
                },
                Token = token
            };

            var messaging = FirebaseMessaging.DefaultInstance;
            try
            {
                var result = await messaging.SendAsync(notification);
                if (result != null)
                {
                    // The notification was sent successfully.
                    // You can handle the success as needed.
                }
                else
                {
                    // Handle the case where FCM SendAsync didn't return a result.
                    // Log an error or handle it as needed.
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur when sending the notification.
                // Log the exception or take appropriate action.
            }
        }

        // Create the notification record in the database, even if the token is missing.
        await _messagingRepository.CreateAsync(entity);
    }

    public async Task SendNotifications(string title, string message, List<int> userIds)
    {
        foreach (int userId in userIds)
        {
            var tokenModel = await _tokenRepository.FirstOrDefaultAsync(x => x.UserId == userId);
            string? token = tokenModel?.Token;

            var entity = new Messaging()
            {
                Title = title,
                Body = message,
                UserId = userId,
                IsRead = false
            };

            if (!string.IsNullOrEmpty(token))
            {
                var notification = new Message()
                {
                    Notification = new Notification
                    {
                        Title = title,
                        Body = message,
                    },
                    Token = token
                };

                var messaging = FirebaseMessaging.DefaultInstance;
                try
                {
                    var result = await messaging.SendAsync(notification);
                    if (result != null)
                    {
                        // The notification was sent successfully.
                        // You can handle the success as needed.
                    }
                    else
                    {
                        // Handle the case where FCM SendAsync didn't return a result.
                        // Log an error or handle it as needed.
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur when sending the notification.
                    // Log the exception or take appropriate action.
                }
            }

            // Create the notification record in the database, even if the token is missing.
            await _messagingRepository.CreateAsync(entity);
        }
    }

    public async Task MarkAsRead(int notificationId)
    {
        var notification = await _messagingRepository.FirstOrDefaultAsync(x => x.Id == notificationId) ?? throw new KeyNotFoundException("Notification is not exist");
        notification.IsRead = true;
        await _messagingRepository.UpdateAsync(notification);
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
            var existToken = await _tokenRepository.FirstOrDefaultAsync(x => x.Token.Equals(token) && x.UserId.Equals(userId));
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

                await _tokenRepository.UpdateAsync(existToken);
            }
        }
    }

    public async Task RemoveToken(int userId, string? token)
    {
        if (token != null)
        {
            var existToken = await _tokenRepository.FirstOrDefaultAsync(x => x.Token.Equals(token) && x.UserId.Equals(userId));
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
