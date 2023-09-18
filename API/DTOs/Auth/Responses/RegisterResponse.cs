using API.Mappings;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Auth.Requests
{
    public class RegisterResponse: IMapFrom<ApplicationUser>
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ReEnteredPassword { get; set; }

    }
}
