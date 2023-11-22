using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Auths
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Password is required")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? ConfirmNewPassword { get; set; }

    }
}
