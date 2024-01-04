using Domain.Models;

namespace API.Services.Interfaces
{
    public interface IFirebaseService
    {
        Task<string> UploadFirebaseAsync(MemoryStream stream, string fileName);
        Task<bool> CreateFirebaseUser(string email, string password);
        Task<bool> UpdateFirebaseUser(string oldMail, string newMail, string? newPassword);
        Task<bool> RemoveFirebaseAccount(int userId);
        Task CreateUserDocument(User user);
        Task UpdateUserDocument(User user);
    }
}
