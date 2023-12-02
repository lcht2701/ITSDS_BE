using Domain.Models.Contracts;
using System.Text.Json.Serialization;

namespace Domain.Models.Tickets;

public partial class Team : BaseEntity
{
    public Team()
    {
        Assignments = new HashSet<Assignment>();
    }

    public string Name { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int? ManagerId { get; set; }

    public int? CategoryId { get; set; }

    public virtual User? Manager { get; set; }
    
    public virtual Category? Category { get; set; }

    [JsonIgnore]
    public virtual ICollection<Assignment>? Assignments { get; set; }
}
