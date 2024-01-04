using API.DTOs.Requests.Auths;
using API.DTOs.Responses.Auths;

namespace API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest model);
        Task<LoginResponse> LoginAdmin(LoginRequest model);
        Task ChangePassword(ChangePasswordRequest model, int userId);
        Task ResetPassword(string email);
    }
}