using Domain.Constants;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Eventing.Reader;
using System.Text.Json.Serialization;

namespace Domain.Models;

public class User : IdentityUser
{
    public User()
    {
        Feedbacks = new HashSet<Feedback>();
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Gender Gender { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public bool isActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid? TeamId { get; set; }

    public virtual Team? Team { get; set; }

    public Guid? CompanyId { get; set; }

    public virtual Company? Company { get; set; }

    [JsonIgnore]
    public virtual ICollection<Feedback> Feedbacks { get; set; }

    [JsonIgnore]
    public virtual ICollection<Ticket> Tickets { get; set; }

}
