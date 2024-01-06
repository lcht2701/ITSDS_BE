using API.DTOs.Responses.Companies;
using API.DTOs.Responses.Teams;
using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.Users;

public class GetUserProfileResponse : IMapFrom<User>
{
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public Gender? Gender { get; set; }
    public Role? Role { get; set; }
    public string? AvatarUrl { get; set; }

    public GetTeamResponse? Team { get; set; }
    public GetCompanyResponse? Company { get; set; }
}
