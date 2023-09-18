using API.Mappings;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Auth.Requests
{
    public class RegisterRequest: IMapTo<ApplicationUser>
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? ReEnteredPassword { get; set; }

    }
}
