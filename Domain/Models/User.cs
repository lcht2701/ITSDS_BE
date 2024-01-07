using System.Text.Json.Serialization;
using Domain.Constants.Enums;
using Domain.Models.Tickets;

namespace Domain.Models;
    
public class User : BaseEntity
{
    public User()
    {
        Tickets = new HashSet<Ticket>();
        TeamMembers = new HashSet<TeamMember>();
        Assignments = new HashSet<Assignment>();
        TicketSolutions = new HashSet<TicketSolution>();
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }
    
    public string Password { get; set; }
    
    public string Email { get; set; }

    public Role Role { get; set; }

    public bool IsActive { get; set; }

    public Gender Gender { get; set; }

    public string? AvatarUrl { get; set; }

    public string? PhoneNumber { get; set; }

    [JsonIgnore] public virtual ICollection<Ticket>? Tickets { get; set; }
    [JsonIgnore] public virtual ICollection<TeamMember>? TeamMembers { get; set; }
    [JsonIgnore] public virtual ICollection<Assignment>? Assignments { get; set; }
    [JsonIgnore] public virtual ICollection<TicketSolution>? TicketSolutions { get; set; }
}