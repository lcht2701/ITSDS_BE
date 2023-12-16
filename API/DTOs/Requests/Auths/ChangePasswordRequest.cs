using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Auths
{
    public class ChangePasswordRequest
    {
        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? ConfirmNewPassword { get; set; }

    }
}
