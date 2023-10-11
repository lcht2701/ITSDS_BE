﻿using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Users
{
    public class UpdateUserRequest : IMapTo<User>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public Gender Gender { get; set; }

        public bool isActive { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }

        public string AvatarUrl { get; set; }
    }
}
