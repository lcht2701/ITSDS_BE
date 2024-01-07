namespace API.Services.Interfaces
{
    public interface IMailService
    {
        Task SendUserCreatedNotification(string fullname, string username, string email, string password, string roleName);

    }
}
