using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.Users;

public class GetUserResponse : IMapFrom<User>
{
    public int? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
    public Role? Role { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public virtual Team? Team { get; set; }
    public virtual Company? Company { get; set; }    
}