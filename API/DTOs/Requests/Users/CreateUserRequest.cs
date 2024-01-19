using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models;
using System.ComponentModel;

namespace API.DTOs.Requests.Users;

public class CreateUserRequest : IMapTo<User>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public Role Role { get; set; }

    //Field included
    [DefaultValue(Gender.Male)]
    public Gender Gender { get; set; }

    public string? AvatarUrl { get; set; }

    public string? PhoneNumber { get; set; }

    public AddCompanyDetailRequest CompanyDetail { get; set; }
}

public class AddCompanyDetailRequest
{
    public int CompanyId { get; set; }

    public int CompanyAddressId { get; set; }
}
