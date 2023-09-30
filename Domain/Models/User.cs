using Domain.Constants;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Eventing.Reader;
using System.Text.Json.Serialization;

namespace Domain.Models;

public class User : BaseEntity
{
    public User()
    {
        MyTickets = new HashSet<Ticket>();
    }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }

    public virtual Role? Role { get; set; }

    public string? PhoneNumber { get; set; }

    public bool? isActive { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public virtual Gender? Gender { get; set; }

    public virtual Team? Team { get; set; }

    public virtual Company? Company { get; set; }

    [JsonIgnore]
    public virtual ICollection<Ticket> MyTickets { get; set; }

}
