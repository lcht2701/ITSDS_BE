using Domain.Models;

namespace API.Services.Interfaces
{
    public interface IFirebaseService
    {
        Task<string> UploadFirebaseAsync(MemoryStream stream, string fileName);
        Task<bool> CreateFirebaseUser(string email, string password);
        Task<bool> UpdateFirebaseUser(string email, string? newPassword);
        Task<bool> RemoveFirebaseAccount(User user);
        Task CreateUserDocument(User user);
        Task UpdateUserDocument(User user);
    }
}
