﻿using API.Mappings;
using Domain.Constants;
using Domain.Models;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.Auths;

public class LoginResponse : IMapFrom<User>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public string AvatarUrl { get; set; }

    public Role Role { get; set; }

    public string PhoneNumber { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string AccessToken { get; set; }
}
