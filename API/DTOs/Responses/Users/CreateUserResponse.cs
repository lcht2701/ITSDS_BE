using API.Mappings;
using Domain.Constants;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Responses.Users
{
    public class CreateUserResponse : IMapFrom<User>
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
