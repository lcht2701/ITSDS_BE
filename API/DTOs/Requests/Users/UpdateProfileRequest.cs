using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models;

namespace API.DTOs.Requests.Users
{
    public class UpdateProfileRequest : IMapTo<User>
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public Gender Gender { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
