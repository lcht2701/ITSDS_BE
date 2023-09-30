using API.Mappings;
using Domain.Constants;
using Domain.Models;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.Auths;

public class LoginResponse
{
    public User User { get; set; }
    public string AccessToken { get; set; }
}

