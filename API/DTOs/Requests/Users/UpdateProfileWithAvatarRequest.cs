using API.Mappings;
using Domain.Constants;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Users
{
    public class UpdateProfileWithAvatarRequest : IMapTo<User>
    {
        public IFormFile? AvatarImage { get; set; }
        public UpdateProfileRequest? UpdateProfileRequest { get; set; }
    }
}
