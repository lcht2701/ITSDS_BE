using API.Mappings;
using Domain.Constants;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Users
{
    public class CreateUserRequest : IMapTo<User>
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 12, ErrorMessage = "Password must be between 12 and 20 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)", ErrorMessage = "Password must contain at least one uppercase, at least one number")]
        public string Password { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required, Range((int)Role.Admin, (int)Role.Accountant)]
        public Role Role { get; set; }
    }
}
