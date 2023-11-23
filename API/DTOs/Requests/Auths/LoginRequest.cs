using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Auths
{
    public class LoginRequest
    {
        public string? Username { get; set; }

        public string? Password { get; set; }

    }
}
