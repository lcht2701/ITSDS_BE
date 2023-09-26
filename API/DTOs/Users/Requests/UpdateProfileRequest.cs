﻿using API.Mappings;
using Domain.Constants;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Users.Requests
{
    public class UpdateProfileRequest : IMapTo<User>
    {
        [Phone]
        public string? PhoneNumber { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public Gender? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}
