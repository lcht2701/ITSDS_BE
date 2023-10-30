using API.Services.Interfaces;
using Domain.Exceptions;
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

    public async Task SendNotification(string message, int userId)
    {
        string title = "ITSDS";
        string token = (await _tokenRepository.FirstOrDefaultAsync(x => x.UserId == userId))?.ToString();
        if (token == null)
        {
            return;
        }
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
        var result = await messaging.SendAsync(notification);
    }

    public async Task CreateNotification(string title, string message, int userId)
    {
        var entity = new Messaging()
        {
            Title = title,
            Body = message,
            UserId = userId,
            IsRead = false
        };
        await _messagingRepository.CreateAsync(entity);
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

    public async Task GetToken(int userId, string token)
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

    public async Task RemoveToken(int userId, string token)
    {
        var existToken = await _tokenRepository.FirstOrDefaultAsync(x => x.Token.Equals(token) && x.UserId.Equals(userId));
        if (existToken != null)
        {
            await _tokenRepository.DeleteAsync(existToken);
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
