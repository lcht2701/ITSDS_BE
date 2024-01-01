using API.DTOs.Requests.Users;
using API.DTOs.Responses.Users;
using Domain.Models;

namespace API.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetManagers();
        Task<List<User>> GetTechnicians();
        Task<List<User>> GetCustomers();
        Task<List<User>> GetAdmins();
        Task<List<User>> GetAccountants();
        Task<List<GetUserResponse>> Get();
        Task<GetUserProfileResponse> GetProfile(int userId);
        Task<GetUserResponse> GetById(int id);
        Task<User> Create(CreateUserRequest model);
        Task<User> Update(int id, UpdateUserRequest model);
        Task Remove(int id);
        Task<User> UpdateProfile(int id, UpdateProfileRequest model);
        Task<string> UploadImageFirebase(int userId, IFormFile file);
        Task<User> UploadAvatarByUrl(int userId, UpdateAvatarUrlRequest model);
        Task CreateUserDocument(User user);
        Task UpdateUserDocument(User user);
    }
}
