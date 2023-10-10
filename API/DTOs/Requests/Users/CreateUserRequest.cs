using API.Mappings;
using Domain.Constants.Enums;
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
        public string Password { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required, Range((int)Role.Admin, (int)Role.Accountant)]
        public Role Role { get; set; }
    }
}
