using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models;
using System.ComponentModel;

namespace API.DTOs.Requests.CompanyMembers;

public class AddAccountInformationRequest : IMapTo<User>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    //Field included
    [DefaultValue(Gender.Male)]
    public Gender Gender { get; set; }

    public string? AvatarUrl { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }
}
